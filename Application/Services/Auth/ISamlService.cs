using System;
using System.Threading.Tasks;

namespace Biobrain.Application.Services.Auth
{
    public interface ISamlService
    {
        Task<SamlAuthnRequestResult> BuildAuthnRequest(Guid schoolId);
        Task<SamlAssertionResult> ProcessSamlResponse(string samlResponse, string relayState);
    }

    public class SamlAuthnRequestResult
    {
        public string RedirectUrl { get; set; }
        public string SamlRequest { get; set; }
        public string RelayState { get; set; }
    }

    public class SamlAssertionResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid SchoolId { get; set; }
    }
}
