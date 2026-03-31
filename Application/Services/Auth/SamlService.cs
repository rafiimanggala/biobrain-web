using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Auth
{
    internal class SamlService : ISamlService
    {
        private readonly IDb _db;
        private readonly ILogger<SamlService> _logger;

        public SamlService(IDb db, ILogger<SamlService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<SamlAuthnRequestResult> BuildAuthnRequest(Guid schoolId)
        {
            var school = await _db.Schools
                .Where(s => s.SchoolId == schoolId && s.SsoEnabled)
                .FirstOrDefaultAsync();

            if (school == null)
                throw new InvalidOperationException("School not found or SSO is not enabled.");

            if (string.IsNullOrWhiteSpace(school.SamlLoginUrl))
                throw new InvalidOperationException("SAML Login URL is not configured for this school.");

            var requestId = $"_bb_{Guid.NewGuid():N}";
            var issueInstant = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            // Build SAML AuthnRequest XML
            var authnRequest = $@"<samlp:AuthnRequest
    xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
    xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
    ID=""{requestId}""
    Version=""2.0""
    IssueInstant=""{issueInstant}""
    ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
    AssertionConsumerServiceURL=""/api/auth/saml/acs"">
    <saml:Issuer>biobrain-sp</saml:Issuer>
    <samlp:NameIDPolicy Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress"" AllowCreate=""true""/>
</samlp:AuthnRequest>";

            // Deflate + Base64 encode for HTTP-Redirect binding
            var samlRequestEncoded = DeflateAndEncode(authnRequest);

            var separator = school.SamlLoginUrl.Contains("?") ? "&" : "?";
            var redirectUrl = $"{school.SamlLoginUrl}{separator}SAMLRequest={Uri.EscapeDataString(samlRequestEncoded)}";

            var relayState = schoolId.ToString();

            return new SamlAuthnRequestResult
            {
                RedirectUrl = redirectUrl + $"&RelayState={Uri.EscapeDataString(relayState)}",
                SamlRequest = samlRequestEncoded,
                RelayState = relayState
            };
        }

        public async Task<SamlAssertionResult> ProcessSamlResponse(string samlResponse, string relayState)
        {
            if (string.IsNullOrWhiteSpace(samlResponse))
                return new SamlAssertionResult { Success = false, Error = "Empty SAML response." };

            if (!Guid.TryParse(relayState, out var schoolId))
                return new SamlAssertionResult { Success = false, Error = "Invalid relay state." };

            var school = await _db.Schools
                .Where(s => s.SchoolId == schoolId && s.SsoEnabled)
                .FirstOrDefaultAsync();

            if (school == null)
                return new SamlAssertionResult { Success = false, Error = "School not found or SSO disabled." };

            try
            {
                var xmlBytes = Convert.FromBase64String(samlResponse);
                var xmlDoc = new XmlDocument { PreserveWhitespace = true };
                xmlDoc.LoadXml(Encoding.UTF8.GetString(xmlBytes));

                // Validate signature if certificate is configured
                if (!string.IsNullOrWhiteSpace(school.SamlCertificate))
                {
                    if (!ValidateSignature(xmlDoc, school.SamlCertificate))
                        return new SamlAssertionResult { Success = false, Error = "SAML signature validation failed." };
                }

                // Extract assertion data
                var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
                nsManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

                // Check status
                var statusCode = xmlDoc.SelectSingleNode("//samlp:StatusCode", nsManager)?.Attributes?["Value"]?.Value;
                if (statusCode != "urn:oasis:names:tc:SAML:2.0:status:Success")
                    return new SamlAssertionResult { Success = false, Error = $"SAML response status: {statusCode}" };

                // Extract NameID (email)
                var nameId = xmlDoc.SelectSingleNode("//saml:NameID", nsManager)?.InnerText;
                if (string.IsNullOrWhiteSpace(nameId))
                    return new SamlAssertionResult { Success = false, Error = "No NameID found in SAML assertion." };

                // Extract attributes (first name, last name)
                var firstName = GetAttributeValue(xmlDoc, nsManager, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")
                             ?? GetAttributeValue(xmlDoc, nsManager, "FirstName")
                             ?? GetAttributeValue(xmlDoc, nsManager, "first_name")
                             ?? "";

                var lastName = GetAttributeValue(xmlDoc, nsManager, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")
                            ?? GetAttributeValue(xmlDoc, nsManager, "LastName")
                            ?? GetAttributeValue(xmlDoc, nsManager, "last_name")
                            ?? "";

                // If names not found in attributes, try to extract from email-like NameID
                if (string.IsNullOrWhiteSpace(firstName) && nameId.Contains("@"))
                {
                    var localPart = nameId.Split('@')[0];
                    var parts = localPart.Split(new[] { '.', '_', '-' }, 2);
                    firstName = parts.Length > 0 ? Capitalize(parts[0]) : "";
                    lastName = parts.Length > 1 ? Capitalize(parts[1]) : "";
                }

                _logger.LogInformation("SAML SSO: Successfully processed assertion for {Email} from school {SchoolId}", nameId, schoolId);

                return new SamlAssertionResult
                {
                    Success = true,
                    Email = nameId.Trim().ToLowerInvariant(),
                    FirstName = firstName,
                    LastName = lastName,
                    SchoolId = schoolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SAML SSO: Error processing SAML response for school {SchoolId}", schoolId);
                return new SamlAssertionResult { Success = false, Error = "Failed to process SAML response." };
            }
        }

        private static bool ValidateSignature(XmlDocument xmlDoc, string certificateBase64)
        {
            try
            {
                var certBytes = Convert.FromBase64String(certificateBase64.Replace("\n", "").Replace("\r", "").Trim());
                using var cert = new X509Certificate2(certBytes);

                var signedXml = new SignedXml(xmlDoc);
                var signatureNodes = xmlDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

                if (signatureNodes.Count == 0)
                    return false;

                signedXml.LoadXml((XmlElement)signatureNodes[0]);
                return signedXml.CheckSignature(cert, true);
            }
            catch
            {
                return false;
            }
        }

        private static string GetAttributeValue(XmlDocument xmlDoc, XmlNamespaceManager nsManager, string attributeName)
        {
            var node = xmlDoc.SelectSingleNode(
                $"//saml:Attribute[@Name='{attributeName}']/saml:AttributeValue", nsManager);
            return node?.InnerText;
        }

        private static string DeflateAndEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using var output = new MemoryStream();
            using (var deflate = new DeflateStream(output, CompressionLevel.Optimal))
            {
                deflate.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        private static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }
    }
}
