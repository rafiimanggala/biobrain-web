using System.Threading.Tasks;
using Biobrain.Application.Interfaces.ExecutionContext;

namespace Biobrain.Application.Security
{
    internal class SecurityService : ISecurityService
    {
        private readonly ISessionContext sessionContext;

        public SecurityService(ISessionContext sessionContext) => this.sessionContext = sessionContext;

        public async Task<IUserSecurityInfo> GetCurrentUserSecurityInfo()
        {
            if (!sessionContext.IsUserAuthenticated) return await CreateAnonymousUserSecurityInfo();
            return await CreateAuthenticatedUserSecurityInfo();
        }
        
        private static Task<AnonymousUserSecurityInfo> CreateAnonymousUserSecurityInfo() => Task.FromResult(new AnonymousUserSecurityInfo());

        private Task<AuthenticatedUserSecurityInfo> CreateAuthenticatedUserSecurityInfo() => Task.FromResult(new AuthenticatedUserSecurityInfo(sessionContext));
    }
}