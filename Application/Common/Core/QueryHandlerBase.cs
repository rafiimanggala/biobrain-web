using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using MediatR;

namespace Biobrain.Application.Common.Core
{
    public abstract class QueryHandlerBase<TRequest, TResponse>(IDb db) : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        protected readonly IDb Db = db;

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}