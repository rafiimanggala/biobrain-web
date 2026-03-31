using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Payments.Models;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Infrastructure.Payments.ErrorHandling;
using Biobrain.Infrastructure.Payments.Models;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PinPayments;

namespace Biobrain.Infrastructure.Payments.Services.Payment
{
	class PaymentService : IPaymentService
	{
		private readonly ILogger _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly IDb _db;

		public PaymentService(ILogger<PaymentService> logger, IDb db, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_db = db;
        }

        /// <summary>
        /// Initiate scheduled payment
        /// </summary>
        /// <param name="scheduledPayment">Payment data</param>
        /// <param name="userPayment">User data</param>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        public async Task<PaymentEntity> Pay(ScheduledPaymentEntity scheduledPayment, UserPaymentDetailsEntity userPayment, PromoCodeEntity promoCode = null)
        {
            PaymentEntity payment;
            var amount = GetAmount(scheduledPayment, promoCode);
			try
            {
	            payment = await CreatePaymentEntity(scheduledPayment, amount);
            }
            catch (Exception e)
            {
	            _logger.LogError(e, "Pay exception");
	            throw;
            }

			try
            {
	            var paymentProvider =
		            GetPaymentProvider.FirstOrDefault(x => x.PaymentMethod == userPayment.PaymentMethod);
	            if (paymentProvider == null)
		            throw new PaymentException($"No payment provider for: {userPayment.PaymentMethod}.");

	            payment = await paymentProvider.PaymentProvider.Pay(amount, scheduledPayment.Currency, userPayment, payment, scheduledPayment.ScheduledPaymentId);


				_logger.LogInformation(
		            $"Start scheduled payment {scheduledPayment.ScheduledPaymentId} for User: {scheduledPayment.UserId}, Amount: {scheduledPayment.Currency}{amount}, Period: {scheduledPayment.Period}, PaymentMethod: {userPayment.PaymentMethod}, PromoCode: {promoCode?.Code} (amount: {promoCode?.Amount}, percent: {promoCode?.Percent})");

			}
			catch (PinException e)
			{
				_logger.LogError(e, $"Pay exception: {scheduledPayment.ScheduledPaymentId}");
				throw new PaymentException(e.Message, e.Error?.Messages?.Select(_ => new ValidationFailure(_.Param, _.Message)));
			}
			catch (Exception e)
            {
	            _logger.LogError(e, $"Pay exception");
	            throw new PaymentException($"Pay exception ScheduledPayment: {scheduledPayment.ScheduledPaymentId}");
            }

            return payment;
        }

        private double GetAmount(ScheduledPaymentEntity scheduledPayment, PromoCodeEntity promoCode)
        {
            if (promoCode == null) return scheduledPayment.Amount;
            if (promoCode.Amount != null)
            {
                var amount = scheduledPayment.Amount - promoCode.Amount.Value;
                return Math.Round(Math.Max(0,amount), 2);
			}
			if (promoCode.Percent != null)
            {
                var amount = scheduledPayment.Amount - (promoCode.Percent.Value * scheduledPayment.Amount / 100);
                return Math.Round(Math.Max(0, amount), 2);
            }
            return scheduledPayment.Amount;
		}

        private async Task<PaymentEntity> CreatePaymentEntity(ScheduledPaymentEntity scheduledPayment, double amount)
		{
			var courses = await _db.ScheduledPaymentCourse.AsNoTracking()
				.Include(_ => _.Course).ThenInclude(_ => _.Subject)
				.Include(_ => _.Course).ThenInclude(_ => _.Curriculum)
				.Where(x => x.ScheduledPaymentId == scheduledPayment.ScheduledPaymentId && x.Status == ScheduledPaymentCourseStatus.Active)
				.Select(x => x.Course).ToListAsync();

			// If scheduled payment not yet saved to DB we can't get courses from DB
			if (!courses.Any() && scheduledPayment.ScheduledPaymentCourses.Any())
			{
				courses = await _db.Courses.AsNoTracking()
					.Include(_ => _.Subject)
					.Include(_ => _.Curriculum)
					.Where(_ => scheduledPayment.ScheduledPaymentCourses.Select(c => c.CourseId).Contains(_.CourseId))
					.ToListAsync();
			}

			return new PaymentEntity
			{
				ScheduledPaymentId = scheduledPayment.ScheduledPaymentId, 
				Status = PaymentStatusEnum.Created,
				Amount = $"{scheduledPayment.Currency}{amount}",
				ProductDescription = string.Join(", ",
					courses.Select(c =>
						$"{c.Subject.Name}{(c.Curriculum.IsGeneric ? "" : $" - Year {c.Year}")}"))
			};
		}

		public async Task<string> GetCardToken(CardModel model, PaymentMethods paymentMethod)
        {
	        var paymentProvider =
		        GetPaymentProvider.FirstOrDefault(x => x.PaymentMethod == paymentMethod);
	        if (paymentProvider == null)
		        throw new PaymentException($"No payment provider for: {paymentMethod}.");
            return await paymentProvider.PaymentProvider.GetCardToken(model);
        }

        public async Task<string> PostCustomerAsync(string email, string cardToken, PaymentMethods paymentMethod)
        {
	        var paymentProvider =
		        GetPaymentProvider.FirstOrDefault(x => x.PaymentMethod == paymentMethod);
	        if (paymentProvider == null)
		        throw new PaymentException($"No payment provider for: {paymentMethod}.");

	        return await paymentProvider.PaymentProvider.PostCustomerAsync(email, cardToken);
        }

        public async Task PutCustomerAsync(string customerToken, string cardToken, string email, PaymentMethods paymentMethod)
        {
	        var paymentProvider =
		        GetPaymentProvider.FirstOrDefault(x => x.PaymentMethod == paymentMethod);
	        if (paymentProvider == null)
		        throw new PaymentException($"No payment provider for: {paymentMethod}.");

	        await paymentProvider.PaymentProvider.PutCustomerAsync(customerToken, cardToken, email);
        }



        private List<PaymentMethodModel> GetPaymentProvider => new List<PaymentMethodModel>
        {
            new() {PaymentMethod = PaymentMethods.PinPayments, PaymentProvider = _serviceProvider.GetService<PinPaymentsProvider>()}
        };
	}
}
