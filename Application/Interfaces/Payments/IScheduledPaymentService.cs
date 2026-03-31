using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Payments.Models;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;

namespace Biobrain.Application.Interfaces.Payments
{
    public interface IScheduledPaymentService
    {
        Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsAsync(Guid userId);

        Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsToNotifyAsync(LastPaymentReviewEntity start,
            LastPaymentReviewEntity end, ScheduledPaymentType type = ScheduledPaymentType.Recurring);

        Task<ScheduledPaymentEntity> AddScheduledPaymentAndPayAsync(ScheduledPaymentEntity model, Guid userId, string country, Guid? promoCodeId, CancellationToken cancellationToken);

        //Task<ScheduledPaymentEntity> UpdateScheduledPaymentAndPayAsync(ScheduledPaymentEntity entity, Guid userId,
	       // string country, CancellationToken cancellationToken);

        Task DeleteScheduledPaymentAsync(Guid paymentId, Guid userId);

        Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsToPayAsync(LastPaymentReviewEntity now, ScheduledPaymentType type = ScheduledPaymentType.Recurring);
        Task<PaymentEntity> Pay(ScheduledPaymentEntity scheduledPayment, Guid? promoCodeId = null);
        List<Price> GetPrices();
        List<SubjectProductModel> GetProductsByCurriculumAvailability(int curriculumCode);
        bool GetYear10CoursesAvailability(string country, int curriculumCode);

        /// <summary>
        /// Get price for scheduled payment
        /// </summary>
        /// <param name="entity">Scheduled payment</param>
        /// <returns>Price</returns>
        public Task<double> GetPrice(ScheduledPaymentEntity entity);
    }
}