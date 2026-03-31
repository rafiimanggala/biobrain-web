using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.SiteIdentity;


namespace Biobrain.Application.Accounts.Services
{
    internal interface ICreateAccountService
    {
        Task<UserEntity> Create(Request request, CancellationToken token);

        internal record Request
        {
            public string Email { get; init; }
            public string UserName { get; init; }
            public string Password { get; init; }
            public ImmutableArray<string> Roles { get; init; }
        }
    }
}
