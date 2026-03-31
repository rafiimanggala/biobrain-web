using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace BiobrainWebAPI.Authorization
{
    public interface IUserAuthorizationService
    {
        Task<ClaimsPrincipal> ExchangeAsync(OpenIddictRequest request);
        //Task ChangePasswordRequest(EmailModelDto model, string host);
        //Task ResetPassword(ResetPasswordDto model);
        //Task ChangePassword(ChangePasswordDto model);
    }
}
