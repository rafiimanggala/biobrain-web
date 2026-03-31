using System;
using System.Threading.Tasks;
using BiobrainWebAPI.Authorization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace BiobrainWebAPI.Controllers
{
    [Route("api/connect")]
    public class AuthorizationController : Controller
    {
        private readonly IUserAuthorizationService userAuthorizationService;

        public AuthorizationController(IUserAuthorizationService userAuthorizationService) => this.userAuthorizationService = userAuthorizationService;

        [HttpPost("token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
	        var request = HttpContext.GetOpenIddictServerRequest() ??
	                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var principal = await userAuthorizationService.ExchangeAsync(request);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        //[Authorize(Roles = RoleNames.PRIMARY_EDUCATOR + "," + RoleNames.SECONDARY_EDUCATOR)]
        //[HttpPost("token/educator"), Produces("application/json")]
        //public async Task<IActionResult> Exchange([FromBody] TeacherDto model)
        //{
        //    var ticket = await userAuthorizationService.AuthenticateEducator(model);
        //    return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        //}
    }
}
