using System.Threading.Tasks;
using Common.Enums;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ISendReviewViewModel
    {
        void SaveRateResult(RateResult result);
        Task SendReview();
    }
}