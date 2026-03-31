using System;
using System.Threading.Tasks;

namespace Biobrain.Application.Accounts.Services
{
    public interface IRefreshClaimsService
    {
        Task RefreshClaims(Guid userId);
    }
}