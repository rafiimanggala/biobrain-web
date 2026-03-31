using System;
using System.Linq;
using System.Security.Claims;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Domain.Constants;
using BiobrainWebAPI.Core.ErrorHandling.Exceptions;
using BiobrainWebAPI.Values;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace BiobrainWebAPI.Authorization
{
    public class SessionContext : ISessionContext
    {
        private readonly ClaimsPrincipal principal;

        public SessionContext(IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor.HttpContext == null)
                throw new InvalidOperationException("'HttpContext' must not be null.");

            principal = contextAccessor.HttpContext.User;
            if (principal == null)
                throw new InvalidOperationException("'HttpContext.User' must not be null.");
        }

        public bool IsUserAuthenticated => principal.Identity?.IsAuthenticated == true;

        public Guid GetUserId()
        {
            var sub = principal.FindFirst("sub");
            if (!Guid.TryParse(sub?.Value, out var userId))
                throw new ServiceException(Errors.InvalidSubject);

            return userId;
        }

        public bool IsUserInRole(string role) => principal.IsInRole(role);

        public bool IsFromSchool(Guid schoolId) =>
	        principal.GetClaim(Constant.ClaimTypes.SchoolId)?.Split(",").Any(x => x == schoolId.ToString()) ?? false;
        //.HasClaim(Constant.ClaimTypes.SchoolId, schoolId.ToString());
    }
}
