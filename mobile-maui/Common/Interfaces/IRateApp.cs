using System.Threading.Tasks;
using Common.Enums;

namespace Common.Interfaces
{
    public interface IRateApp
    {
        Task<RateResult> RateApp();
    }
}