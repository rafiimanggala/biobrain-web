using System.Threading.Tasks;
using Biobrain.Application.Common.Models;
using Biobrain.Domain.Entities.AccessCodes;

namespace Biobrain.Application.Services.Domain.AccessCode
{
    public interface IAccessCodeService
    {
        Task<GenerateCodeResult> TryGetNewAccessCode();
        string GetBatchHeader(AccessCodeBatchEntity batch);
    }
}