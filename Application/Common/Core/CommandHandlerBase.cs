using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using MediatR;

namespace Biobrain.Application.Common.Core
{
    public abstract class CommandHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {
        protected readonly IDb Db;

        protected CommandHandlerBase(IDb db) => Db = db;

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}