using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Application.Interfaces.Payments;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Payment;
using Biobrain.Infrastructure.Payments.ErrorHandling;
using BiobrainWebAPI.Values;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Infrastructure.Payments.Services.Hosted
{
    public class PaymentHostedService : IHostedService, IDisposable
    {
	    private const int PaymentPeriodSeconds = 3600;
	    private const int StartOffsetSeconds = 120;
        private const int PaymentAtOneTime = 100;
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public PaymentHostedService(ILogger<PaymentHostedService> logger, IServiceProvider services, IConfiguration configuration)
        {
            this._logger = logger;
            this._services = services;
            this._configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
			_logger.LogInformation("Payment Service is starting.");

			_timer = new Timer(MakePayments, null, TimeSpan.FromSeconds(StartOffsetSeconds),
				TimeSpan.FromSeconds(PaymentPeriodSeconds));

			_logger.LogInformation("Payment Service is started.");
			return Task.CompletedTask;
		}

        private async void MakePayments(object state)
        {
            _logger.LogInformation("Payments review.");
            using var scope = _services.CreateScope();

            var scopedNotificationService =
                scope.ServiceProvider.GetRequiredService<INotificationService>();
            var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;
            try
            {

                var scopedScheduledPaymentService =
                    scope.ServiceProvider.GetRequiredService<IScheduledPaymentService>();
                var scopedPaymentDateService =
                    scope.ServiceProvider.GetRequiredService<IPaymentDateService>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();

                var now = new LastPaymentReviewEntity
                    { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow) };
                var scheduledPayments =
                    (await scopedScheduledPaymentService.GetScheduledPaymentsToPayAsync(now)).ToList();

                _logger.LogInformation($"{scheduledPayments.Count} payments to pay.");

                var scopedPaymentHistoryService =
                    scope.ServiceProvider.GetRequiredService<IPaymentHistoryService>();

                // ToDo redo?
                if (scheduledPayments.Count > PaymentAtOneTime)
                {
                    var lastReview = await scopedPaymentHistoryService.GetLastPaymentReview();
                    var message =
                        $"Too much payments to pay!\n DateTime(utc): {DateTime.UtcNow:G}\n Messages count: {scheduledPayments.Count}\n Date token now: {now.PayDate}\n Date token previous: {lastReview.PayDate}";
                    _logger.LogWarning(message);
                    await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                        "Warning - PaymentHostedService - To many payments", message));
                }

                var successPayments = 0;
                foreach (var scheduledPayment in scheduledPayments)
                {
                    try
                    {
                        // ToDo: if StoppedByUser then delete
                        switch (scheduledPayment.Status)
                        {
                            case ScheduledPaymentStatus.PaymentFailed:
                            case ScheduledPaymentStatus.Success:
                                // Check for stopped by user subjects
                                scheduledPayment.ScheduledPaymentCourses.ForEach(x =>
                                {
                                    if (x.Status == ScheduledPaymentCourseStatus.StoppedByUser)
                                        x.Status = ScheduledPaymentCourseStatus.Inactive;
                                });

                                // Update subscription amount using new subject number
                                //scheduledPayment.Amount =
                                // Prices.GetCost(
                                //  scheduledPayment.ScheduledPaymentCourses.Count(x =>
                                //   x.Status == ScheduledPaymentCourseStatus.Active), scheduledPayment.Period, scheduledPayment.Currency);

                                var payment = await scopedScheduledPaymentService.Pay(scheduledPayment);

                                if (scheduledPayment.Status == ScheduledPaymentStatus.PaymentFailed)
                                {
                                    // ToDo remove related classes
                                    await RemoveRelatedClasses(scope, scheduledPayment);
                                }

                                await db.Payment.AddAsync(payment);
                                successPayments++;
                                break;
                            case ScheduledPaymentStatus.StoppedByUser:
                                scheduledPayment.Status = ScheduledPaymentStatus.Inactive;
                                scheduledPayment.DeletedAt = DateTime.UtcNow;
                                //ToDo: Delete related classes
                                await RemoveRelatedClasses(scope, scheduledPayment);
                                break;
                            default:
                                _logger.LogWarning(
                                    $"Scheduled payment with unhandled status: {scheduledPayment.Status}");
                                break;
                        }
                    }
                    catch (PaymentException e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - PaymentHostedService - PaymentException",
                            $"Exception: {e.ToString()} \n Errors: {string.Join('\n', e.Errors?.Select(x => x.ErrorMessage ?? e.Message) ?? new List<string>())}"));
                    }
                    catch (Exception e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - PaymentHostedService - Exception",
                            $"{e.ToString()}"));
                    }
                    finally
                    {
                        await scopedPaymentHistoryService.AddPaidScheduledPayment(scheduledPayment.ScheduledPaymentId);
                    }
                }

                await db.SaveChangesAsync();
                _logger.LogInformation($"{successPayments} payments paid.");

                await NotifyAboutRenew(scope);
                await CheckFreeTrials(scope);
                await CheckAccessCodes(scope);
                await CheckVouchers(scope);

                await scopedPaymentHistoryService.AddLastPaymentReview(now.PayDate);
                await scopedPaymentHistoryService.CleanOldHistoryEntities();


                _logger.LogInformation("End payments review.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Hosted service exception");
                await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                    "Error - PaymentHostedService - PaymentException",
                    $"Exception: {e.ToString()}"));
            }
        }

        private async Task RemoveRelatedClasses(IServiceScope scope, ScheduledPaymentEntity scheduledPayment)
        {
            var db = scope.ServiceProvider.GetRequiredService<IDb>();
            var schoolClassesToRemove = await db.SchoolClassStudents
                .Include(_ => _.SchoolClass).ThenInclude(_ => _.School)
                .Where(_ => _.StudentId == scheduledPayment.UserId 
                            && _.SchoolClass != null
                            && _.SchoolClass.School != null
                            && _.SchoolClass.School.UseAccessCodes)
                .Where(_ => scheduledPayment.ScheduledPaymentCourses.Any(sp =>
                    sp.CourseId == _.SchoolClass.CourseId))
                .ToListAsync();
            if(schoolClassesToRemove.Any())
                db.SchoolClassStudents.RemoveRange(schoolClassesToRemove);
        }

        public async Task NotifyAboutRenew(IServiceScope scope)
        {
            try
            {
                var scopedScheduledPaymentService =
                    scope.ServiceProvider.GetRequiredService<IScheduledPaymentService>();
                var scopedPaymentDateService =
                    scope.ServiceProvider.GetRequiredService<IPaymentDateService>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;

                var start = new LastPaymentReviewEntity { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow.AddDays(7).Date) };
                var end = new LastPaymentReviewEntity { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow.AddDays(8).Date.AddTicks(-1)) };
                var scheduledPayments = (await scopedScheduledPaymentService.GetScheduledPaymentsToNotifyAsync(start, end)).ToList();

                _logger.LogTrace($"{scheduledPayments.Count} payments to notify.");

                var scopedPaymentHistoryService =
                    scope.ServiceProvider.GetRequiredService<IPaymentHistoryService>();

                var scopedNotificationService =
                 scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                foreach (var scheduledPayment in scheduledPayments)
                {
                    try
                    {
                        switch (scheduledPayment.Status)
                        {
                            case ScheduledPaymentStatus.PaymentFailed:
                            case ScheduledPaymentStatus.Success:
                                await scopedNotificationService.Send(new SubscriptionRenewNotification(
                                    scheduledPayment.User.Email,
                                    scheduledPayment.User.GetFirstName()
                                ));
                                await scopedPaymentHistoryService.AddPaidScheduledPayment(scheduledPayment.ScheduledPaymentId);
                                break;

                            case ScheduledPaymentStatus.StoppedByUser:
                            case ScheduledPaymentStatus.Created:
                                break;
                            default:
                                _logger.LogWarning($"Subscription with unhandled status: {scheduledPayment.Status}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - Renew Notification - Exception",
                            $"{e.ToString()}"));
                    }
                }

                await db.SaveChangesAsync();

                _logger.LogTrace("End free trials check.");

            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Free Trial Service failed.");
            }
        }

        public async Task CheckFreeTrials(IServiceScope scope)
        {
            try
            {
                var scopedScheduledPaymentService =
                    scope.ServiceProvider.GetRequiredService<IScheduledPaymentService>();
                var scopedPaymentDateService =
                    scope.ServiceProvider.GetRequiredService<IPaymentDateService>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;

                var now = new LastPaymentReviewEntity { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow) };
                var scheduledPayments = (await scopedScheduledPaymentService.GetScheduledPaymentsToPayAsync(now, ScheduledPaymentType.FreeTrial)).ToList();

                _logger.LogTrace($"{scheduledPayments.Count} free trials to stop.");

                var scopedNotificationService =
                 scope.ServiceProvider.GetRequiredService<INotificationService>();
                var scopedPaymentHistoryService =
                    scope.ServiceProvider.GetRequiredService<IPaymentHistoryService>();

                var stoppedFreeTrials = 0;
                foreach (var scheduledPayment in scheduledPayments)
                {
                    try
                    {
                        switch (scheduledPayment.Status)
                        {
                            case ScheduledPaymentStatus.PaymentFailed:
                            case ScheduledPaymentStatus.Success:
                            case ScheduledPaymentStatus.StoppedByUser:
                            case ScheduledPaymentStatus.Created:
                                // Check for stopped by user subjects
                                scheduledPayment.ScheduledPaymentCourses.ForEach(x =>
                                {
                                    x.Status = ScheduledPaymentCourseStatus.Inactive;
                                });
                                scheduledPayment.Status = ScheduledPaymentStatus.Inactive;
                                scheduledPayment.DeletedAt = DateTime.UtcNow;
                                stoppedFreeTrials++;
                                db.Update(scheduledPayment);
                                break;
                            default:
                                _logger.LogWarning($"Free trial with unhandled status: {scheduledPayment.Status}");
                                break;
                        }
                        await scopedPaymentHistoryService.AddPaidScheduledPayment(scheduledPayment.ScheduledPaymentId);
                    }
                    catch (Exception e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - FreeTrialHostedService - Exception",
                            $"{e.ToString()}"));
                    }
                }

                await db.SaveChangesAsync();
                _logger.LogInformation($"{stoppedFreeTrials} free trials stopped.");


                _logger.LogTrace("End free trials check.");

            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Free Trial Service failed.");
            }
        }

        public async Task CheckAccessCodes(IServiceScope scope)
        {
            try
            {
                var scopedScheduledPaymentService =
                    scope.ServiceProvider.GetRequiredService<IScheduledPaymentService>();
                var scopedPaymentDateService =
                    scope.ServiceProvider.GetRequiredService<IPaymentDateService>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;

                var now = new LastPaymentReviewEntity { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow) };
                var scheduledPayments = (await scopedScheduledPaymentService.GetScheduledPaymentsToPayAsync(now, ScheduledPaymentType.AccessCode)).ToList();

                _logger.LogTrace($"{scheduledPayments.Count} access codes to stop to stop.");

                var scopedNotificationService =
                 scope.ServiceProvider.GetRequiredService<INotificationService>();
                var scopedPaymentHistoryService =
                    scope.ServiceProvider.GetRequiredService<IPaymentHistoryService>();

                var stoppedFreeTrials = 0;
                foreach (var scheduledPayment in scheduledPayments)
                {
                    // Access Code without ExpiryDate should work for 15 month (more than year). Algorithm to select payments work for subscriptions < 1 year.
                    // To handle this issue check subscription to be created more than year ago
                    // If not do this subscription will stop after 3 month
                    if (scheduledPayment.ExpiryDate == null && (DateTime.UtcNow - scheduledPayment.CreatedAt).TotalDays < 366) continue;

                    // ExpiryDate not meet yet -> do nothing
                    if(scheduledPayment.ExpiryDate != null && scheduledPayment.ExpiryDate <= DateTime.UtcNow) continue;

                    try
                    {
                        switch (scheduledPayment.Status)
                        {
                            case ScheduledPaymentStatus.PaymentFailed:
                            case ScheduledPaymentStatus.Success:
                            case ScheduledPaymentStatus.StoppedByUser:
                            case ScheduledPaymentStatus.Created:
                                // Check for stopped by user subjects
                                scheduledPayment.ScheduledPaymentCourses.ForEach(x =>
                                {
                                    x.Status = ScheduledPaymentCourseStatus.Inactive;
                                });
                                scheduledPayment.Status = ScheduledPaymentStatus.Inactive;
                                scheduledPayment.DeletedAt = DateTime.UtcNow;
                                var schoolClassesToRemove = await db.SchoolClassStudents
                                    .Include(_ => _.SchoolClass).ThenInclude(_ => _.School)
                                    .Where(_ => _.StudentId == scheduledPayment.UserId &&
                                                _.SchoolClass.School.UseAccessCodes)
                                    .Where(_ => scheduledPayment.ScheduledPaymentCourses.Any(sp =>
                                        sp.CourseId == _.SchoolClass.CourseId))
                                    .ToListAsync();
                                db.SchoolClassStudents.RemoveRange(schoolClassesToRemove);

                                stoppedFreeTrials++;
                                db.Update(scheduledPayment);
                                break;
                            default:
                                _logger.LogWarning($"Access code subscription with unhandled status: {scheduledPayment.Status}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - AccessCodeSubscriptionHostedService - Exception",
                            $"{e.ToString()}"));
                    }
                }

                await db.SaveChangesAsync();
                _logger.LogInformation($"{stoppedFreeTrials} access codes subscriptions stopped.");


                _logger.LogTrace("End access codes check.");

            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Free Trial Service failed.");
            }
        }

        public async Task CheckVouchers(IServiceScope scope)
        {
            try
            {
                var scopedScheduledPaymentService =
                    scope.ServiceProvider.GetRequiredService<IScheduledPaymentService>();
                var scopedPaymentDateService =
                    scope.ServiceProvider.GetRequiredService<IPaymentDateService>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var adminEmail = _configuration.GetSection(ConfigurationSections.ErrorEmail).Value;

                var now = new LastPaymentReviewEntity { PayDate = scopedPaymentDateService.GetPaydate(DateTime.UtcNow) };
                var scheduledPayments = (await scopedScheduledPaymentService.GetScheduledPaymentsToPayAsync(now, ScheduledPaymentType.Voucher)).ToList();

                _logger.LogTrace($"{scheduledPayments.Count} access codes to stop to stop.");

                var scopedNotificationService =
                 scope.ServiceProvider.GetRequiredService<INotificationService>();
                var scopedPaymentHistoryService =
                    scope.ServiceProvider.GetRequiredService<IPaymentHistoryService>();

                var stoppedVouchers = 0;
                foreach (var scheduledPayment in scheduledPayments)
                {
                    // Access Code without ExpiryDate should work for 15 month (more than year). Algorithm to select payments work for subscriptions < 1 year.
                    // To handle this issue check subscription to be created more than year ago
                    // If not do this subscription will stop after 3 month
                    //if (scheduledPayment.ExpiryDate == null && (DateTime.UtcNow - scheduledPayment.CreatedAt).TotalDays < 366) continue;

                    // ExpiryDate not meet yet -> do nothing
                    if (scheduledPayment.ExpiryDate != null && scheduledPayment.ExpiryDate <= DateTime.UtcNow) continue;

                    try
                    {
                        switch (scheduledPayment.Status)
                        {
                            case ScheduledPaymentStatus.PaymentFailed:
                            case ScheduledPaymentStatus.Success:
                            case ScheduledPaymentStatus.StoppedByUser:
                            case ScheduledPaymentStatus.Created:
                                // Check for stopped by user subjects
                                scheduledPayment.ScheduledPaymentCourses.ForEach(x =>
                                {
                                    x.Status = ScheduledPaymentCourseStatus.Inactive;
                                });
                                scheduledPayment.Status = ScheduledPaymentStatus.Inactive;
                                scheduledPayment.DeletedAt = DateTime.UtcNow;
                                var schoolClassesToRemove = await db.SchoolClassStudents
                                    .Include(_ => _.SchoolClass).ThenInclude(_ => _.School)
                                    .Where(_ => _.StudentId == scheduledPayment.UserId &&
                                                _.SchoolClass.School.UseAccessCodes)
                                    .Where(_ => scheduledPayment.ScheduledPaymentCourses.Any(sp =>
                                        sp.CourseId == _.SchoolClass.CourseId))
                                    .ToListAsync();
                                db.SchoolClassStudents.RemoveRange(schoolClassesToRemove);

                                stoppedVouchers++;
                                db.Update(scheduledPayment);
                                break;
                            default:
                                _logger.LogWarning($"Voucher subscription with unhandled status: {scheduledPayment.Status}");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        await scopedNotificationService.Send(new PaymentErrorAdminNotification(adminEmail,
                            "Error - VoucherSubscriptionHostedService - Exception",
                            $"{e.ToString()}"));
                    }
                }

                await db.SaveChangesAsync();
                _logger.LogInformation($"{stoppedVouchers} voucher subscriptions stopped.");


                _logger.LogTrace("End voucher check.");

            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Voucher Service failed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payment Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}