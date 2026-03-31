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
    public class WelcomeEmailService : IHostedService, IDisposable
    {
	    private const int ServicePeriodHours = 1;
	    private const int CheckUsersForDays = 3;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private Timer _timer;

        public WelcomeEmailService(ILogger<TempFileDeleterService> logger, 
                                    IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("WelcomeEmailService starting.");

            try
            {
                _timer = new Timer(SendWelcomeEmails, null, TimeSpan.FromMinutes(5), TimeSpan.FromHours(ServicePeriodHours));

                _logger.LogTrace("WelcomeEmailService is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "WelcomeEmailService was not started.");
            }

            return Task.CompletedTask;
        }

        private async void SendWelcomeEmails(object state)
        {
            _logger.LogTrace("WelcomeEmailService checking.");

            try
            {
                using var scope = _serviceProvider.CreateScope();

                var scopedNotificationService =
                    scope.ServiceProvider.GetRequiredService<INotificationService>();

                var siteUrls = scope.ServiceProvider.GetRequiredService<ISiteUrls>();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();

                var sentEmails = await db.TempHistory.Where(_ => _.Type == Constant.HistoryType.WelcomeEmail)
                    .Select(_ => _.RefId).ToListAsync();
                var usersToSend = await db.Users
                    .Include(_ => _.Student)
                    .Where(_ =>
                        _.Student != null && _.Student.CreatedAt.AddDays(CheckUsersForDays) > DateTime.UtcNow
                                          && !sentEmails.Contains(_.Id))
                    .ToListAsync();

                foreach (var user in usersToSend)
                {
                    await scopedNotificationService.Send(new WelcomeStudentNotification(user.Email, user.GetFirstName(), siteUrls.Login()));
                    db.TempHistory.Add(new TempHistoryEntity{Type = Constant.HistoryType.WelcomeEmail, DaysAlive = CheckUsersForDays + 1, RefId = user.Id, TempHistoryId = Guid.NewGuid()});
                }

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "WelcomeEmailService failed.");
            }
            _logger.LogTrace("WelcomeEmailService checking end.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("WelcomeEmailService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
