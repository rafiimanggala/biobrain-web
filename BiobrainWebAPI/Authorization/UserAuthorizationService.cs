using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobrain.Application.Accounts.Services;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;
using BiobrainWebAPI.Core.ErrorHandling.Exceptions;
using BiobrainWebAPI.Values;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace BiobrainWebAPI.Authorization
{
    public class UserAuthorizationService(SignInManager<UserEntity> signInManager,
                                          UserManager<UserEntity> userManager,
                                          IDb db,
                                          ILogger<UserAuthorizationService> logger,
                                          IHttpContextAccessor contextAccessor,
                                          IRefreshClaimsService refreshClaimsService)
        : IUserAuthorizationService
    {
        private readonly SignInManager<UserEntity> signInManager = signInManager;
        private readonly UserManager<UserEntity> userManager = userManager;
        private readonly IDb db = db;
        private readonly IHttpContextAccessor contextAccessor = contextAccessor;
        private readonly ILogger<UserAuthorizationService> logger = logger;
        private readonly IRefreshClaimsService _refreshClaimsService = refreshClaimsService;

        public async Task<ClaimsPrincipal> ExchangeAsync(OpenIddictRequest request)
        {
            return request.GrantType switch
            {
                OpenIddictConstants.GrantTypes.Password => await ExchangePasswordGrantType(request),
                OpenIddictConstants.GrantTypes.RefreshToken => await ExchangeRefreshTokenGrantType(request),
                _ => throw new ServiceException(OpenIddictConstants.Errors.UnsupportedGrantType)
            };
        }

        private async Task<ClaimsPrincipal> ExchangePasswordGrantType(OpenIddictRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (!await ValidateUser(user))
                throw new ServiceException(Errors.UserNamePasswordInvalid);

            logger.LogInformation($"{Strings.PasswordLogin}: {user.UserName} ({user.Id})");

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                logger.LogError($"User Name/password is invalid: {request.Username}");
                throw new ServiceException(Errors.UserNamePasswordInvalid);
            }

            if (request.HasParameter("usercode"))
            {
                var login = (string)request.GetParameter("usercode").Value;
                user = await userManager.FindByNameAsync(login);
            }

            user.LoginCount++;
            await userManager.UpdateAsync(user);

            await _refreshClaimsService.RefreshClaims(user.Id);
            var principal = await signInManager.CreateUserPrincipalAsync(user);

            //var ticket = await authenticationTickerBuilder
            //    .BuildTicketAsync(principal, request.GetScopes());

            // Set the list of scopes granted to the client application.
            // Note: the offline_access scope must be granted
            // to allow OpenIddict to return a refresh token.
            principal.SetScopes(new[]
            {
	            OpenIddictConstants.Scopes.OpenId,
	            OpenIddictConstants.Scopes.Email,
	            OpenIddictConstants.Scopes.Profile,
	            OpenIddictConstants.Scopes.OfflineAccess,
	            OpenIddictConstants.Scopes.Roles
            }.Intersect(request.GetScopes()));

            var loginCountClaim = new Claim("login_count", user.LoginCount.ToString());
            loginCountClaim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            principal.AddIdentity(new System.Security.Claims.ClaimsIdentity(new[] { loginCountClaim }));

            foreach (var claim in principal.Claims)
            {
	            claim.SetDestinations(GetDestinations(claim, principal));
            }

            return principal;
        }
        
        private async Task<ClaimsPrincipal> ExchangeRefreshTokenGrantType(OpenIddictRequest request)
        {
            // Retrieve the claims principal stored in the refresh token.
            var info = await contextAccessor.HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var user = await userManager.GetUserAsync(info.Principal);
            if (!await ValidateUser(user))
                throw new ServiceException(Errors.InvalidRefreshToken);

            logger.LogInformation($"{Strings.RefreshLogin}: {user.UserName} ({user.Id})");

            if (!await signInManager.CanSignInAsync(user))
                throw new ServiceException(Errors.UserCantLogin);

            await _refreshClaimsService.RefreshClaims(user.Id);
            var principal = await signInManager.CreateUserPrincipalAsync(user);

            foreach (var claim in principal.Claims)
            {
	            claim.SetDestinations(GetDestinations(claim, principal));
            }

            return principal;
        }

        private async Task<bool> ValidateUser(UserEntity user)
        {
	        if (user == null || user.DeletedAt != null) return false;

	        if (await userManager.IsInRoleAsync(user, Constant.Roles.Student))
	        {
		        var student = await db.Students.FindAsync(user.Id);
		        if (student == null) return false;
		        //if (student.SchoolId == null) return true;

		        //var school = await db.Schools.Where(x => x.SchoolId == student.SchoolId).FirstOrDefaultAsync();
		        //if (school == null || school.Status == Constant.SchoolStatus.Archive) return false;
	        }else if (await userManager.IsInRoleAsync(user, Constant.Roles.Teacher))
	        {
		        var teacher = await db.Teachers.FindAsync(user.Id);
		        if (teacher == null) return false;

		        //var school = await db.Schools.Where(x => x.SchoolId == teacher.SchoolId).FirstOrDefaultAsync();
		        //if (school == null || school.Status == Constant.SchoolStatus.Archive) return false;
            }
		    
	        return true;
        }

        private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
	        // Note: by default, claims are NOT automatically included in the access and identity tokens.
	        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
	        // whether they should be included in access tokens, in identity tokens or in both.

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

		        // Never include the security stamp in the access and identity tokens, as it's a secret value.
		        case "AspNet.Identity.SecurityStamp": yield break;

		        default:
			        yield return OpenIddictConstants.Destinations.AccessToken;
			        yield break;
	        }
        }
    }
}
