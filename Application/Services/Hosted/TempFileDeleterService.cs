using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Content.Services.ContentCacheService;
using BiobrainWebAPI.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Hosted
{
    public class TempFileDeleterService : IHostedService, IDisposable
    {
	    private const int ServicePeriodDays = 1;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private Timer _timer;

        public TempFileDeleterService(ILogger<TempFileDeleterService> logger, 
                                    IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("File Deleter Service is starting.");

            try
            {
                _timer = new Timer(DeleteFiles, null, TimeSpan.Zero, TimeSpan.FromDays(ServicePeriodDays));

                _logger.LogInformation("File Deleter Service is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "File Deleter Service was not started.");
            }

            return Task.CompletedTask;
        }

        private async void DeleteFiles(object state)
        {
	        try
	        {
		        using var scope = _serviceProvider.CreateScope();

		        var contentCacheService = scope.ServiceProvider.GetRequiredService<IContentCacheService>();
		        var deleted = await contentCacheService.DeleteOldFiles();
		        deleted += CleanDirectory(Path.Combine(Directory.GetCurrentDirectory(),
			        _configuration.GetSection(ConfigurationSections.CacheFolder).Value, AppSettings.ReportFolderLink));

                if (deleted > 0)
					_logger.LogInformation($"File Deleter Service delete {deleted} files.");

            }
	        catch (Exception e)
	        {
		        _logger.LogError(default, e, "Delete files failed.");
	        }
        }

        private int CleanDirectory(string path, int hourAlive = 1)
        {
	        var counter = 0;
            if (!Directory.Exists(path)) return 0;
	        foreach (var filePath in Directory.EnumerateFiles(path))
	        {
		        if ((DateTime.UtcNow - File.GetCreationTimeUtc(filePath)).TotalHours > hourAlive)
		        {
			        File.Delete(filePath);
			        counter++;
		        }
	        }

	        return counter;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message Sender Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
