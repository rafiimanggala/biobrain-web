using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Behaviours;
using Biobrain.Application.Security;

namespace Biobrain.Application.Common.Core
{
    internal abstract class PermissionCheckBase<TRequest> : IPermissionCheck<TRequest>
    {
        protected readonly ISecurityService SecurityService;

        protected PermissionCheckBase(ISecurityService securityService) => SecurityService = securityService;

        public async Task<bool> CanExecute(TRequest request, CancellationToken cancellationToken)
        {
            var userSecurityInfo = await SecurityService.GetCurrentUserSecurityInfo();

            return CanExecute(request, userSecurityInfo);
        }
        
        protected abstract bool CanExecute(TRequest request, IUserSecurityInfo user);
    }
}