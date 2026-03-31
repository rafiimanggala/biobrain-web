using System;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Application.Interfaces.Payments
{
    public interface IPaymentHistoryService
    {
        /// <summary>
        /// Add paid scheduled payment (to avoid double payment)
        /// </summary>
        /// <param name="scheduledPaymentId">ScheduledPaymentId</param>
        /// <returns></returns>
        Task AddPaidScheduledPayment(Guid scheduledPaymentId);

        /// <summary>
        /// Add last payment review (to start from it next time)
        /// </summary>
        /// <param name="paydate">current pay date in format: mmddhh</param>
        /// <returns></returns>
        Task AddLastPaymentReview(int paydate);

        /// <summary>
        /// Get last payment review
        /// </summary>
        /// <returns>LastPaymentReviewEntity</returns>
        Task<LastPaymentReviewEntity> GetLastPaymentReview();

        /// <summary>
        /// Clean old history fields
        /// </summary>
        /// <returns></returns>
        Task CleanOldHistoryEntities();
    }
}