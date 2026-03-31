using MediatR;

namespace Biobrain.Application.Common.Core
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {

    }
}