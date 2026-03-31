using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Services.Auth;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.SiteIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/auth/saml")]
    [ApiController]
    public class SamlAuthController : Controller
    {
        private readonly ISamlService _samlService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IRefreshClaimsService _refreshClaimsService;
        private readonly IDb _db;
        private readonly ILogger<SamlAuthController> _logger;

        public SamlAuthController(
            ISamlService samlService,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            IRefreshClaimsService refreshClaimsService,
            IDb db,
            ILogger<SamlAuthController> logger)
        {
            _samlService = samlService;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshClaimsService = refreshClaimsService;
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Initiates SAML SSO login. Redirects user to the school's IdP.
        /// GET /api/auth/saml/login?schoolId=GUID
        /// </summary>
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] Guid schoolId)
        {
            try
            {
                var result = await _samlService.BuildAuthnRequest(schoolId);
                return Redirect(result.RedirectUrl);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lookup school by name/code for SSO. Returns schoolId if SSO is enabled.
        /// GET /api/auth/saml/lookup?schoolName=XYZ
        /// </summary>
        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string schoolName)
        {
            if (string.IsNullOrWhiteSpace(schoolName))
                return BadRequest(new { error = "School name is required." });

            var school = await _db.Schools
                .Where(s => s.SsoEnabled && s.Name.ToLower().Contains(schoolName.ToLower().Trim()))
                .Select(s => new { s.SchoolId, s.Name })
                .FirstOrDefaultAsync();

            if (school == null)
                return NotFound(new { error = "No SSO-enabled school found with that name." });

            return Ok(school);
        }

        /// <summary>
        /// SAML Assertion Consumer Service. Receives SAML Response from IdP (HTTP-POST binding).
        /// POST /api/auth/saml/acs
        /// </summary>
        [HttpPost("acs")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Acs([FromForm] string SAMLResponse, [FromForm] string RelayState)
        {
            var assertion = await _samlService.ProcessSamlResponse(SAMLResponse, RelayState);

            if (!assertion.Success)
            {
                _logger.LogWarning("SAML ACS failed: {Error}", assertion.Error);
                return Redirect($"/login?ssoError={Uri.EscapeDataString(assertion.Error)}");
            }

            // Find or create user
            var user = await _userManager.FindByEmailAsync(assertion.Email);
            if (user == null)
            {
                user = await CreateSsoUser(assertion);
            }
            else if (user.DeletedAt != null)
            {
                return Redirect($"/login?ssoError={Uri.EscapeDataString("Account has been deactivated.")}");
            }

            // Ensure user is linked to the school
            await EnsureSchoolMembership(user, assertion.SchoolId);

            // Update login count
            user.LoginCount++;
            await _userManager.UpdateAsync(user);

            // Refresh claims & create principal
            await _refreshClaimsService.RefreshClaims(user.Id);
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Set scopes
            principal.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Scopes.Roles
            });

            // Set claim destinations
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Issue token via OpenIddict
            var result = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // For SAML POST binding, we redirect to frontend with a marker.
            // The frontend will call a token exchange endpoint.
            // Simpler approach: issue a short-lived SSO code, redirect to frontend.
            var ssoCode = Guid.NewGuid().ToString("N");
            SsoCodeStore.Store(ssoCode, user.Id, TimeSpan.FromMinutes(2));

            _logger.LogInformation("SAML SSO: User {Email} authenticated via school {SchoolId}", assertion.Email, assertion.SchoolId);

            return Redirect($"/login?ssoCode={ssoCode}");
        }

        /// <summary>
        /// Exchange SSO code for JWT tokens. Called by frontend after SAML redirect.
        /// POST /api/auth/saml/exchange
        /// </summary>
        [HttpPost("exchange")]
        public async Task<IActionResult> Exchange([FromBody] SsoExchangeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Code))
                return BadRequest(new { error = "SSO code is required." });

            var userId = SsoCodeStore.Consume(request.Code);
            if (userId == null)
                return BadRequest(new { error = "Invalid or expired SSO code." });

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
                return BadRequest(new { error = "User not found." });

            await _refreshClaimsService.RefreshClaims(user.Id);
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            principal.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.OfflineAccess,
                OpenIddictConstants.Scopes.Roles
            });

            var loginCountClaim = new Claim("login_count", user.LoginCount.ToString());
            loginCountClaim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            principal.AddIdentity(new ClaimsIdentity(new[] { loginCountClaim }));

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        private async Task<UserEntity> CreateSsoUser(SamlAssertionResult assertion)
        {
            var user = new UserEntity
            {
                UserName = assertion.Email,
                Email = assertion.Email,
                EmailConfirmed = true
            };

            // Create user without password (SSO-only)
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create SSO user: {errors}");
            }

            // Default role: Student (can be changed by school admin later)
            await _userManager.AddToRoleAsync(user, Constant.Roles.Student);

            // Create student record
            var student = new Biobrain.Domain.Entities.Student.StudentEntity
            {
                StudentId = user.Id,
                FirstName = assertion.FirstName,
                LastName = assertion.LastName
            };
            await _db.Students.AddAsync(student);
            await _db.SaveChangesAsync();

            _logger.LogInformation("SAML SSO: Created new user {Email} for school {SchoolId}", assertion.Email, assertion.SchoolId);

            return user;
        }

        private async Task EnsureSchoolMembership(UserEntity user, Guid schoolId)
        {
            var isStudent = await _userManager.IsInRoleAsync(user, Constant.Roles.Student);
            var isTeacher = await _userManager.IsInRoleAsync(user, Constant.Roles.Teacher);

            if (isStudent)
            {
                var exists = await _db.SchoolStudents
                    .AnyAsync(ss => ss.StudentId == user.Id && ss.SchoolId == schoolId);
                if (!exists)
                {
                    await _db.SchoolStudents.AddAsync(new SchoolStudentEntity
                    {
                        StudentId = user.Id,
                        SchoolId = schoolId
                    });
                    await _db.SaveChangesAsync();
                }
            }
            else if (isTeacher)
            {
                var exists = await _db.SchoolTeachers
                    .AnyAsync(st => st.TeacherId == user.Id && st.SchoolId == schoolId);
                if (!exists)
                {
                    await _db.SchoolTeachers.AddAsync(new SchoolTeacherEntity
                    {
                        TeacherId = user.Id,
                        SchoolId = schoolId
                    });
                    await _db.SaveChangesAsync();
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Name:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.Email:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case OpenIddictConstants.Claims.Role:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                case "AspNet.Identity.SecurityStamp":
                    yield break;

                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
    }

    public class SsoExchangeRequest
    {
        public string Code { get; set; }
    }

    /// <summary>
    /// Simple in-memory store for short-lived SSO codes.
    /// Maps a one-time code to a UserId for SAML callback flow.
    /// </summary>
    public static class SsoCodeStore
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (Guid UserId, DateTime Expiry)> _codes = new();

        public static void Store(string code, Guid userId, TimeSpan ttl)
        {
            _codes[code] = (userId, DateTime.UtcNow + ttl);
            CleanExpired();
        }

        public static Guid? Consume(string code)
        {
            if (_codes.TryRemove(code, out var entry) && entry.Expiry > DateTime.UtcNow)
                return entry.UserId;
            return null;
        }

        private static void CleanExpired()
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _codes)
            {
                if (kvp.Value.Expiry < now)
                    _codes.TryRemove(kvp.Key, out _);
            }
        }
    }
}
