using MediatR;

namespace Biobrain.Application.Common.Core
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
}