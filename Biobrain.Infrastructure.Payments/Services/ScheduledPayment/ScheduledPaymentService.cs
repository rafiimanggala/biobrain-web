using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Application.Payments.Models;
using Biobrain.Application.Specifications;
using Biobrain.Application.Values;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Infrastructure.Payments.ErrorHandling;
using Biobrain.Infrastructure.Payments.Values;
using BiobrainWebAPI.Values;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Biobrain.Infrastructure.Payments.Services.ScheduledPayment
{
    public class ScheduledPaymentService(ILogger<ScheduledPaymentService> logger,
                                         IDb db,
                                         IPaymentService paymentService,
                                         INotificationService notificationService,
                                         IPaymentDateService paymentDateService,
                                         IConfiguration configuration,
                                         IPaymentHistoryService paymentHistoryService)
        : IScheduledPaymentService
    {
        private static LastPaymentReviewEntity StartYear => new() { PayDate = 010100 };
        private static LastPaymentReviewEntity EndYear => new() { PayDate = 123123 };
        private static LastPaymentReviewEntity StartMonth => new() { PayDate = 0100 };
        private static LastPaymentReviewEntity EndMonth => new() { PayDate = 3123 };

        private readonly IPaymentService _paymentService = paymentService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IPaymentDateService _paymentDateService = paymentDateService;
        private readonly IDb _db = db;
        private readonly ILogger<ScheduledPaymentService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IPaymentHistoryService _paymentHistoryService = paymentHistoryService;

        /// <summary>
        /// Get scheduled payments for user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>Scheduled payments for user</returns>
        public async Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsAsync(Guid userId)
        {
            var payments = await _db.ScheduledPayment
                .Where(x => x.UserId == userId)
                .Where(x => x.DeletedAt == null)
                .Include(x => x.User)
                .ToListAsync();
            return payments;
        }

        /// <summary>
        /// Get Scheduled payment that need to pay
        /// </summary>
        /// <param name="now">Current payment mark</param>
        /// <returns>Scheduled payments to pay</returns>
        public async Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsToNotifyAsync(LastPaymentReviewEntity start, LastPaymentReviewEntity end, ScheduledPaymentType type = ScheduledPaymentType.Recurring)
        {
            var prev = start;
            var now = end;
            var payments = new List<ScheduledPaymentEntity>();

            // Year payments
            // If prevDate > nowDate than it's end of year inside interval
            // We need to split interval to two:
            // From prevDate to EndYear
            // From StartYear to nowDate
            if (now.PayDate < prev.PayDate)
            {
                var endYearPayments = await GetScheduledPaymentsAsync(prev, EndYear, type);
                var startYearPayments = await GetScheduledPaymentsAsync(StartYear, now, type);
                payments.AddRange(endYearPayments);
                payments.AddRange(startYearPayments);
            }
            else
            {
                payments = await GetScheduledPaymentsAsync(prev, now, type);
            }

            // Month payments
            //var monthNow = new LastPaymentReviewEntity { PayDate = now.PayDate % 10000 };
            //var monthPrev = new LastPaymentReviewEntity { PayDate = prev.PayDate % 10000 };
            // If prevDate > nowDate than it's end of month inside interval
            // We need to split interval to two:
            // From prevDate to EndMonth
            // From StartMonth to nowDate
            //if (monthNow.PayDate < monthPrev.PayDate)
            //{
            //    var endYearPayments = await GetScheduledMonthPaymentsAsync(monthPrev, EndMonth, type);
            //    var startYearPayments = await GetScheduledMonthPaymentsAsync(StartMonth, monthNow, type);
            //    payments.AddRange(endYearPayments);
            //    payments.AddRange(startYearPayments);
            //}
            //else
            //{
            //    payments.AddRange(await GetScheduledMonthPaymentsAsync(monthPrev, monthNow, type));
            //}


            return payments;
        }

        /// <summary>
        /// Get Scheduled payment that need to pay
        /// </summary>
        /// <param name="now">Current payment mark</param>
        /// <returns>Scheduled payments to pay</returns>
        public async Task<IEnumerable<ScheduledPaymentEntity>> GetScheduledPaymentsToPayAsync(LastPaymentReviewEntity now, ScheduledPaymentType type = ScheduledPaymentType.Recurring)
        {
            var prev = (await _db.LastPaymentReview.AnyAsync())
                ? (await _db.LastPaymentReview.OrderBy(x => x.CreatedAt).LastAsync())
                : new LastPaymentReviewEntity {PayDate = now.PayDate - 1};
            var payments = new List<ScheduledPaymentEntity>();

            // Year payments
            // If prevDate > nowDate than it's end of year inside interval
            // We need to split interval to two:
            // From prevDate to EndYear
            // From StartYear to nowDate
            if (now.PayDate < prev.PayDate)
            {
                var endYearPayments = await GetScheduledPaymentsAsync(prev, EndYear, type);
                var startYearPayments = await GetScheduledPaymentsAsync(StartYear, now, type);
                payments.AddRange(endYearPayments);
                payments.AddRange(startYearPayments);
            }
            else
            {
                payments = await GetScheduledPaymentsAsync(prev, now, type);
            }

            // Month payments
            var monthNow = new LastPaymentReviewEntity{PayDate = now.PayDate % 10000 };
            var monthPrev = new LastPaymentReviewEntity {PayDate = prev.PayDate % 10000 };
            // If prevDate > nowDate than it's end of month inside interval
            // We need to split interval to two:
            // From prevDate to EndMonth
            // From StartMonth to nowDate
            if (monthNow.PayDate < monthPrev.PayDate)
            {
	            var endYearPayments = await GetScheduledMonthPaymentsAsync(monthPrev, EndMonth, type);
	            var startYearPayments = await GetScheduledMonthPaymentsAsync(StartMonth, monthNow, type);
	            payments.AddRange(endYearPayments);
	            payments.AddRange(startYearPayments);
            }
            else
            {
	            payments.AddRange(await GetScheduledMonthPaymentsAsync(monthPrev, monthNow, type));
            }


            return payments;
        }

        private async Task<List<ScheduledPaymentEntity>> GetScheduledPaymentsAsync(LastPaymentReviewEntity from,
            LastPaymentReviewEntity to, ScheduledPaymentType type)
        {
            // Leap
            if (DateTime.IsLeapYear(DateTime.UtcNow.Year))
                return await GetScheduledPaymentsAsync(x => x.LeapPayDate > from.PayDate && x.LeapPayDate <= to.PayDate, PaymentPeriods.Yearly, type);
            //await _db.ScheduledPayment
            //   .Where(x => x.DeletedAt == null && x.Status != ScheduledPaymentStatus.Created && x.Status != ScheduledPaymentStatus.Inactive)
            //   .Include(x => x.User)
            //   // Dates restriction
            //   .Where(x =>  x.LeapPayDate > from.PayDate && x.LeapPayDate <= to.PayDate)
            //   // Not recently paid
            //   .Where(x => (!_db.LastPaidScheduledPayment.Select(y => y.ScheduledPaymentId)
            //    .Contains(x.ScheduledPaymentId)))
            //   .ToListAsync();

            // Not leap
            return await GetScheduledPaymentsAsync(x => x.PayDate > from.PayDate && x.PayDate <= to.PayDate, PaymentPeriods.Yearly, type);

                //_db.ScheduledPayment
                //.Where(x => x.DeletedAt == null && x.Status != ScheduledPaymentStatus.Created && x.Status != ScheduledPaymentStatus.Inactive)
                //.Include(x => x.User)
                //// Dates restriction
                //.Where(x => x.PayDate > from.PayDate && x.PayDate <= to.PayDate)
                //// Not recently paid
                //.Where(x => (!_db.LastPaidScheduledPayment.Select(y => y.ScheduledPaymentId)
	               // .Contains(x.ScheduledPaymentId)))
                //.ToListAsync();
        }

        private async Task<List<ScheduledPaymentEntity>> GetScheduledMonthPaymentsAsync(LastPaymentReviewEntity from,
	        LastPaymentReviewEntity to, ScheduledPaymentType type)
        {
	        // Leap
	        if (DateTime.IsLeapYear(DateTime.UtcNow.Year))
		        return await GetScheduledPaymentsAsync(x => x.LeapPayDate % 10000 > from.PayDate && x.LeapPayDate % 10000 <= to.PayDate, PaymentPeriods.Monthly, type);

	        // Not leap
	        return await GetScheduledPaymentsAsync(x => x.PayDate % 10000 > from.PayDate && x.PayDate % 10000 <= to.PayDate, PaymentPeriods.Monthly, type);
        }

        private async Task<List<ScheduledPaymentEntity>> GetScheduledPaymentsAsync(System.Linq.Expressions.Expression<Func<ScheduledPaymentEntity, bool>> predicate, PaymentPeriods period, ScheduledPaymentType type)
        {
	        return await _db.ScheduledPayment
		        .Where(x => x.DeletedAt == null 
		                    && x.Status != ScheduledPaymentStatus.Created && x.Status != ScheduledPaymentStatus.Inactive 
		                    && x.Period == period
                            && x.Type == type)
		        .Include(x => x.User)
		        .Include(x => x.ScheduledPaymentCourses)
		        .ThenInclude(x => x.Course)
		        .ThenInclude(x => x.Subject)
		        // Dates restriction
		        .Where(predicate)
		        // Not recently paid
		        .Where(x => (!_db.LastPaidScheduledPayment.Select(y => y.ScheduledPaymentId)
			        .Contains(x.ScheduledPaymentId)))
		        .ToListAsync();
        }

        /// <summary>
        /// Add scheduled payment
        /// </summary>
        /// <param name="entity">Scheduled payments</param>
        /// <param name="userId">User id</param>
        /// <param name="country"></param>
        /// <param name="promoCodeId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Added Scheduled payments model</returns>
        public async Task<ScheduledPaymentEntity> AddScheduledPaymentAndPayAsync(ScheduledPaymentEntity entity, Guid userId, string country, Guid? promoCodeId, CancellationToken cancellationToken)
        {
	        using (await _db.BeginTransactionAsync(cancellationToken))
	        {
		        try
		        {
			        var currency = CountryCurrency.Get(country);
			        if (string.IsNullOrEmpty(currency.Key))
				        throw new PaymentException("Can't detect currency for selected country");

			        //Calc to pay days (for leap and not leap year) to handle 29 Feb issues
			        entity.UserId = userId;
			        entity.PayDate = _paymentDateService.GetNotLeapPaydate(DateTime.UtcNow);
			        entity.LeapPayDate = _paymentDateService.GetLeapPaydate(DateTime.UtcNow);
			        entity.Currency = currency.Key;
			        entity.Amount = await GetPrice(entity);
                    entity.Status = ScheduledPaymentStatus.Created;
                    
                    var addedEntity = _db.ScheduledPayment.Add(entity).Entity;
			        await _db.SaveChangesAsync(cancellationToken);
			        _logger.LogInformation(
				        $"Add scheduled payment: EntityId: {addedEntity.ScheduledPaymentId} User:{userId}, Amount: {addedEntity.Amount}, Courses: {string.Join(',', addedEntity.ScheduledPaymentCourses?.Select(x => x.CourseId) ?? new List<Guid>())}");

			        var payment = await Pay(addedEntity, promoCodeId);
			        _db.Payment.Add(payment);
			        await _db.LastPaidScheduledPayment.AddAsync(new LastPaidScheduledPaymentEntity
			        {
				        ScheduledPaymentId = addedEntity.ScheduledPaymentId,
			        }, cancellationToken);

                    var freeTrial = await _db.ScheduledPayment.Where(_ => _.Type == ScheduledPaymentType.FreeTrial && _.Status != ScheduledPaymentStatus.Inactive)
                        .FirstOrDefaultAsync(cancellationToken);
                    if(freeTrial != null)
                        freeTrial.Status = ScheduledPaymentStatus.Inactive;
                    await _db.SaveChangesAsync(cancellationToken);

			        await _db.CommitTransactionAsync();
			        return addedEntity;
		        }
		        catch (Exception)
		        {
			        await _db.RollbackTransactionAsync();
			        throw;
		        }
	        }
        }

        ///// <summary>
        ///// Update scheduled payment
        ///// </summary>
        ///// <param name="entity">Scheduled payments</param>
        ///// <param name="userId">User id</param>
        ///// <param name="country"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns>Added Scheduled payments model</returns>
        //public async Task<ScheduledPaymentEntity> UpdateScheduledPaymentAndPayAsync(ScheduledPaymentEntity entity, Guid userId, string country, CancellationToken cancellationToken)
        //{
        //    using (await _db.BeginTransactionAsync(cancellationToken))
        //    {
        //        try
        //        {
        //            var currency = CountryCurrency.Get(country);
        //            if (string.IsNullOrEmpty(currency.Key))
        //                throw new PaymentException("Can't detect currency for selected country");

        //            //Calc to pay days (for leap and not leap year) to handle 29 Feb issues
        //            entity.UserId = userId;
        //            entity.PayDate = _paymentDateService.GetNotLeapPaydate(DateTime.UtcNow);
        //            entity.LeapPayDate = _paymentDateService.GetLeapPaydate(DateTime.UtcNow);
        //            entity.Currency = currency.Key;
        //            entity.Amount = await GetPrice(entity);
        //            entity.Status = ScheduledPaymentStatus.Created;


        //            var addedEntity = _db.ScheduledPayment.Update(entity).Entity;
        //            await _db.SaveChangesAsync(cancellationToken);
        //            _logger.LogInformation(
        //                $"Update scheduled payment: EntityId: {addedEntity.ScheduledPaymentId} User:{userId}, Amount: {addedEntity.Amount}, Courses: {string.Join(',', addedEntity.ScheduledPaymentCourses.Select(x => x.CourseId))}");

        //            var payment = await Pay(addedEntity);
        //            _db.Payment.Add(payment);
        //            await _db.LastPaidScheduledPayment.AddAsync(new LastPaidScheduledPaymentEntity
        //            {
	       //             ScheduledPaymentId = addedEntity.ScheduledPaymentId,
        //            }, cancellationToken);
        //            await _db.SaveChangesAsync(cancellationToken);

        //            await _db.CommitTransactionAsync();
        //            return addedEntity;
        //        }
        //        catch (Exception e)
        //        {
        //            await _db.RollbackTransactionAsync();
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Get price for scheduled payment
        /// </summary>
        /// <param name="entity">Scheduled payment</param>
        /// <returns>Price</returns>
        public async Task<double> GetPrice(ScheduledPaymentEntity entity)
        {
            double count = 0;
            var activeCourse = await _db.Courses.AsNoTracking()
                .Where(CourseSpec.ByIds(entity.ScheduledPaymentCourses.Where(_ => _.Status == ScheduledPaymentCourseStatus.Active).Select(_ => _.CourseId)))
                .ToListAsync();
            activeCourse.ForEach(_ =>
                                 {
                                     // Additional courses: 2 for price of 1
                                     count += _.Group switch
                                     {
                                         CourseGroup.Undefined or CourseGroup.Main => 1,
                                         CourseGroup.Additional => 0.5,
                                         _ => throw new ArgumentOutOfRangeException(nameof(_.Group))
                                     };
                                 });
            var subjectsCount = (int)Math.Round(count, MidpointRounding.AwayFromZero);
            return Prices.GetCost(subjectsCount, entity.Period, entity.Currency);
        }

        ///// <summary>
        ///// Update scheduled payment
        ///// </summary>
        ///// <param name="entity">Scheduled payments</param>
        ///// <param name="userId">User id</param>
        ///// <param name="country"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns>Added Scheduled payments model</returns>
        //public async Task<ScheduledPaymentEntity> UpdateScheduledPaymentAsync(ScheduledPaymentEntity entity, Guid userId, string country, CancellationToken cancellationToken)
        //{
        //    using (await _db.BeginTransactionAsync(cancellationToken))
        //    {
        //        try
        //        {
        //            var currency = CountryCurrency.Get(country);
        //            if (string.IsNullOrEmpty(currency.Key))
        //                throw new PaymentException("Can't detect currency for selected country");

        //            //Calc to pay days (for leap and not leap year) to handle 29 Feb issues
        //            entity.UserId = userId;
        //            entity.PayDate = _paymentDateService.GetNotLeapPaydate(DateTime.UtcNow);
        //            entity.LeapPayDate = _paymentDateService.GetLeapPaydate(DateTime.UtcNow);
        //            entity.Amount = GetCost(entity);
        //            entity.Currency = currency.Key;


        //            var addedEntity = _db.ScheduledPayment.Add(entity).Entity;
        //            await _db.SaveChangesAsync(cancellationToken);
        //            _logger.LogInformation(
        //                $"Update scheduled payment: EntityId: {addedEntity.ScheduledPaymentId} User:{userId}, Amount: {addedEntity.Amount}, Courses: {string.Join(',', addedEntity.ScheduledPaymentCourses.Select(x => x.CourseId))}");

        //            await _db.CommitTransactionAsync();
        //            return addedEntity;
        //        }
        //        catch (Exception e)
        //        {
        //            await _db.RollbackTransactionAsync();
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Pay Scheduled payments
        /// </summary>
        /// <param name="scheduledPayment">Scheduled payments to pay</param>
        /// <param name="promoCodeId"></param>
        public async Task<PaymentEntity> Pay(ScheduledPaymentEntity scheduledPayment, Guid? promoCodeId = null)
        {
	        var userPayment = await _db.UserPaymentDetails.Where(x => x.UserId == scheduledPayment.UserId)
		        .Include(x => x.User)
		        .FirstOrDefaultAsync();

            if (userPayment == null)
	        {
                var e = new PaymentException($"You should populate billing details first.");
                _logger.LogError(e, $"No payment method for this user: {scheduledPayment.UserId}.");
                throw e;
	        }

            var promoCode = await GetPromoCodeByCodeCommand(promoCodeId, scheduledPayment.UserId);
	        var payment = await _paymentService.Pay(scheduledPayment, userPayment, promoCode);

            var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;
            switch (payment.Status)
            {
                case PaymentStatusEnum.Created:
                    if(!string.IsNullOrEmpty(adminEmail))
		                await _notificationService.Send(new PaymentErrorAdminNotification(adminEmail, "Payment Status Error",
			                "'Created' after charge was processed"));
                    _logger.LogWarning("Payment Status Error: 'Created' after charge was processed");
					break;
                case PaymentStatusEnum.ChargeFailed:
	                scheduledPayment.Status = ScheduledPaymentStatus.PaymentFailed;
	                if (!string.IsNullOrEmpty(adminEmail))
		                await _notificationService.Send(new PaymentErrorAdminNotification(adminEmail, "Payment Failed",
			                payment.FailedPayload));
	                _logger.LogWarning("Payment Status Error: 'Created' after charge was processed");
                    //await messageService.SaveMessageAsync(
                    //	strings.Messages.DonationDeclinedMessage(scheduledPayment.User.FirstName,
                    //		scheduledPayment.Charity.Title, scheduledPayment.Amount.ToString(".##")),
                    //	strings.Messages.DonationDeclinedMessageSubject(scheduledPayment.Charity.Title),
                    //	scheduledPayment.User.Email);
                    break;
                case PaymentStatusEnum.ChargeSuccess:
                    // Set new paydate if PaymentFailed, because next payment period should start now
                    if (scheduledPayment.Status == ScheduledPaymentStatus.PaymentFailed)
	                {
		                scheduledPayment.PayDate = _paymentDateService.GetNotLeapPaydate(DateTime.UtcNow);
		                scheduledPayment.LeapPayDate = _paymentDateService.GetLeapPaydate(DateTime.UtcNow);
                    }

	                scheduledPayment.Status = ScheduledPaymentStatus.Success;
                    if (promoCode != null)
                    {
                        _db.UserPromoCodes.Add(new UserPromoCodeEntity{UserId = scheduledPayment.UserId, PromoCodeId = promoCode.PromoCodeId, UserPromoCodeId = Guid.NewGuid()});
                    }
	                //ToDo: Add email notifications
                    //var date = TimeZoneInfo.ConvertTimeFromUtc(payment.CreatedAt,
                    //    TZConvert.GetTimeZoneInfo(scheduledPayment.User.TimeZoneId));
                    //await messageService.SaveMessageAsync(
                    //    strings.Messages.MadeDonationMessage(scheduledPayment.User.FirstName,
                    //        scheduledPayment.Charity.Title, scheduledPayment.Charity.Abn,
                    //        scheduledPayment.Amount.ToString(".##"), date.ToString("ddd MMM dd hh:mm:ss zz yyyy"),
                    //        $"{payment.PaymentId}{date:hhmmssFFddmmyyyy}"),
                    //    strings.Messages.MadeDonationMessageSubject(scheduledPayment.Charity.Title),
                    //    scheduledPayment.User.Email);
                    break;
                default:
	                //ToDo: Add email notifications
                    //await messageService.SaveMessageAsync(
                    //    strings.Messages.PaymentErrorAdmindMessage(
                    //        $"Status error (unhandled status) - {payment.Status}",
                    //        JsonConvert.SerializeObject(payment)),
                    //    strings.Messages.PaymentErrorAdminMessageSubject, appSettings.Emailing.AdminEmail);
                    break;
            }

            await _db.SaveChangesAsync();
            return payment;
        }

        private async Task<PromoCodeEntity> GetPromoCodeByCodeCommand(Guid? promoCodeId, Guid userId)
        {
            if (promoCodeId == null) return null;

            var userPromoCode = await _db.UserPromoCodes.Where(_ => _.UserId == userId && _.PromoCodeId == promoCodeId)
                .FirstOrDefaultAsync();
            if (userPromoCode != null) return null;
            return await _db.PromoCodes.Where(PromoCodeSpec.ById(promoCodeId.Value)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Delete Scheduled payment
        /// </summary>
        /// <param name="paymentId">Scheduled payment to delete id</param>
        /// <param name="userId">User id</param>
        public async Task DeleteScheduledPaymentAsync(Guid paymentId, Guid userId)
        {
            var paymentEntity = await _db.ScheduledPayment.Where(x => x.ScheduledPaymentId == paymentId).FirstOrDefaultAsync() ??
                                throw new ScheduledPaymentException($"Scheduled payment with id: {paymentId} not found");

            if (paymentEntity.UserId != userId)
                throw new ScheduledPaymentException($"Scheduled payment with id: {paymentId} not belong to the user: {userId}");

            paymentEntity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Delete scheduled payment: EntityId: {paymentEntity.ScheduledPaymentId} User:{userId}, Amount: {paymentEntity.Amount}");
        }

        public List<Price> GetPrices() => Prices.Values;

        public List<SubjectProductModel> GetProductsByCurriculumAvailability(int curriculumCode)
            => CoursesAvailability.Products.FirstOrDefault(_ => _.CurriculumCode == curriculumCode)?.Subjects ?? [];

        public bool GetYear10CoursesAvailability(string country, int curriculumCode) => true; // CoursesAvailability.Year10CoursesCountries.Contains(country) && CoursesAvailability.Year10CoursesCurricula.Contains(curriculumCode);
    }
}