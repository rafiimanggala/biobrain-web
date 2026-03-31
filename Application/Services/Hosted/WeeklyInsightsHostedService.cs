using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Services.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Hosted
{
    public class WeeklyInsightsHostedService : IHostedService, IDisposable
    {
        private const int CheckIntervalHours = 6;
        private const DayOfWeek SendDay = DayOfWeek.Monday;

        private readonly ILogger<WeeklyInsightsHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private Timer _timer;
        private DateTime _lastSentDate = DateTime.MinValue;

        public WeeklyInsightsHostedService(
            ILogger<WeeklyInsightsHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("WeeklyInsightsHostedService starting.");

            try
            {
                _timer = new Timer(
                    ExecuteAsync,
                    null,
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromHours(CheckIntervalHours));

                _logger.LogTrace("WeeklyInsightsHostedService is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "WeeklyInsightsHostedService was not started.");
            }

            return Task.CompletedTask;
        }

        private async void ExecuteAsync(object state)
        {
            var now = DateTime.UtcNow;

            if (now.DayOfWeek != SendDay)
            {
                return;
            }

            if (_lastSentDate.Date == now.Date)
            {
                return;
            }

            _logger.LogInformation("WeeklyInsightsHostedService triggered on {Date}", now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var insightsService = scope.ServiceProvider
                    .GetRequiredService<IPerformanceInsightsService>();

                await insightsService.SendWeeklyInsightsAsync();

                _lastSentDate = now;
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "WeeklyInsightsHostedService failed.");
            }

            _logger.LogInformation("WeeklyInsightsHostedService completed.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("WeeklyInsightsHostedService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
