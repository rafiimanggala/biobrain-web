using System;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobrain.Infrastructure.Payments.Services.PaymentHistory
{
    /// <summary>
    /// Service works with payment service history tables.
    /// </summary>
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private const int NumberOfLastReviewEntities = 1000;
        private const int NumberOfDaysToSavePayments = 5;

        private readonly ILogger _logger;
        private readonly IDb _db;

        public PaymentHistoryService(ILogger<PaymentHistoryService> logger, IDb db)
        {
	        _logger = logger;
	        _db = db;
        }

        /// <summary>
        /// Add paid scheduled payment (to avoid double payment)
        /// </summary>
        /// <param name="scheduledPaymentId">ScheduledPaymentId</param>
        /// <returns></returns>
        public async Task AddPaidScheduledPayment(Guid scheduledPaymentId)
        {
            await _db.LastPaidScheduledPayment.AddAsync(new LastPaidScheduledPaymentEntity
            {
                ScheduledPaymentId = scheduledPaymentId,
            });
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Add last payment review (to start from it next time)
        /// </summary>
        /// <param name="paydate">current pay date in format: mmddhh</param>
        /// <returns></returns>
        public async Task AddLastPaymentReview(int paydate)
        {
            await _db.LastPaymentReview.AddAsync(new LastPaymentReviewEntity
            {
                PayDate = paydate,
            });
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Get last payment review
        /// </summary>
        /// <returns>LastPaymentReviewEntity</returns>
        public async Task<LastPaymentReviewEntity> GetLastPaymentReview()
        {
            var lastReview = await _db.LastPaymentReview.OrderBy(x => x.CreatedAt).LastAsync();
            return lastReview;
        }

        /// <summary>
        /// Clean old history fields
        /// </summary>
        /// <returns></returns>
        public async Task CleanOldHistoryEntities()
        {
            var reviewsToDelete = await _db.LastPaymentReview.OrderByDescending(x => x.CreatedAt)
                .Skip(NumberOfLastReviewEntities).ToListAsync();

            _db.RemoveRange(reviewsToDelete);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Deleted {reviewsToDelete.Count} rows from LastPaymentReview");
            var date = DateTime.UtcNow.Subtract(TimeSpan.FromDays(NumberOfDaysToSavePayments));

            var lasPaymentsToDelete = await _db.LastPaidScheduledPayment
                .Where(x => x.CreatedAt < date)
                .ToListAsync();

            _db.RemoveRange(lasPaymentsToDelete);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Deleted {lasPaymentsToDelete.Count} rows from LastPaidScheduledPayment");
        }
    }
}