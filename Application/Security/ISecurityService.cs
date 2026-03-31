using System.Threading.Tasks;

namespace Biobrain.Application.Security
{
    internal interface ISecurityService
    {
        Task<IUserSecurityInfo> GetCurrentUserSecurityInfo();
    }
}