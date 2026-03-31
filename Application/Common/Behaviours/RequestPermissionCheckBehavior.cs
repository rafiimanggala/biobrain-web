using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Exceptions;
using MediatR;

namespace Biobrain.Application.Common.Behaviours
{
    public interface IPermissionCheck<in TRequest>
    {
        Task<bool> CanExecute(TRequest request, CancellationToken cancellationToken);
    }

    internal class RequestPermissionCheckBehavior<TRequest, TResponse>(IEnumerable<IPermissionCheck<TRequest>> permissionChecks)
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IPermissionCheck<TRequest>> _permissionChecks = permissionChecks;

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var checks = _permissionChecks.Select(_ => _.CanExecute(request, cancellationToken));
            var results = await Task.WhenAll(checks);

            if (results.Any(_ => _ == false))
                throw new PermissionDeniedException();

            return await next();
        }
    }
}
