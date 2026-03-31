using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Interfaces.Notifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Hosted
{
    public class FreeTrialCheckerService : IHostedService, IDisposable
    {
	    private const int ServicePeriodHours = 1;
	    private const int SendSecondEmailAfterDays = 7;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private Timer _timer;

        public FreeTrialCheckerService(ILogger<TempFileDeleterService> logger, 
                                    IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Free Trial Service is starting.");

            try
            {
                _timer = new Timer(CheckFreeTrials, null, TimeSpan.FromMinutes(5), TimeSpan.FromHours(ServicePeriodHours));

                _logger.LogTrace("Free Trial Service is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Free Trial Service was not started.");
            }

            return Task.CompletedTask;
        }

        private async void CheckFreeTrials(object state)
        {
            _logger.LogTrace("Free Trial Service checking.");

            try
            {
                using var scope = _serviceProvider.CreateScope();

                var scopedNotificationService =
                    scope.ServiceProvider.GetRequiredService<INotificationService>();
                var siteUrls = scope.ServiceProvider.GetRequiredService<ISiteUrls>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var sentTrials = await db.TempHistory.Where(_ => _.Type == Constant.HistoryType.FreeTrialEmail)
                    .Select(_ => _.RefId).ToListAsync();
                var subscriptionsToSend = await db.ScheduledPayment
                    .Include(_ => _.User)
                    .Where(_ =>
                        _.Type == ScheduledPaymentType.FreeTrial && _.Status != ScheduledPaymentStatus.Inactive
                        && _.CreatedAt.AddDays(SendSecondEmailAfterDays) < DateTime.UtcNow
                        && !sentTrials.Contains(_.ScheduledPaymentId))
                    .ToListAsync();

                foreach (var subscription in subscriptionsToSend)
                {
                    await scopedNotificationService.Send(new FreeTrialSecondNotification(subscription.User.Email, subscription.User.GetFirstName(), siteUrls.Login()));
                    db.TempHistory.Add(new TempHistoryEntity{Type = Constant.HistoryType.FreeTrialEmail, DaysAlive = 14, RefId = subscription.ScheduledPaymentId, TempHistoryId = Guid.NewGuid()});
                }

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Free Trial check failed.");
            }
            _logger.LogTrace("Free Trial Service checking end.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Free Trial  Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
