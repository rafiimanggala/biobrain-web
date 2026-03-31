using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Hosted
{
    public class TempHistoryCleanerService : IHostedService, IDisposable
    {
	    private const int ServicePeriodDays = 1;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private Timer _timer;

        public TempHistoryCleanerService(ILogger<TempFileDeleterService> logger, 
                                    IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Temp History Cleaner Service is starting.");

            try
            {
                _timer = new Timer(CleanHistory, null, TimeSpan.FromMinutes(5), TimeSpan.FromDays(ServicePeriodDays));

                _logger.LogTrace("Temp History Cleaner Service  is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Temp History Cleaner Service  was not started.");
            }

            return Task.CompletedTask;
        }

        private async void CleanHistory(object state)
        {
            _logger.LogTrace("Temp History Cleaner Service cleaning.");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IDb>();
                var entriesToDelete = await db.TempHistory
                    .Where(_ => _.CreatedAt.AddDays(_.DaysAlive) < DateTime.UtcNow).ToListAsync();
                db.RemoveRange(entriesToDelete);
                await db.SaveChangesAsync();
                _logger.LogTrace($"Temp History Cleaner Service cleaning end ({entriesToDelete.Count} deleted).");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "History cleaning failed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Temp History Cleaner Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
