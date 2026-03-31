using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobrain.Infrastructure.Notifications
{
    public class MessageSenderService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private Timer _timer;
        private SmtpServerConfiguration SmtpServerConfiguration { get; set; }

        public MessageSenderService(ILogger<MessageSenderService> logger, 
                                    IServiceProvider serviceProvider, 
                                    IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Message Sender Service is starting.");

            try
            {
                SmtpServerConfiguration = GetSmtpConfiguration();
                _timer = new Timer(SendMessages, null, TimeSpan.Zero, TimeSpan.FromSeconds(SmtpServerConfiguration.SenderPeriod));

                _logger.LogInformation("Message Sender Service is started.");
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, "Message Sender Service was not started.");
            }

            return Task.CompletedTask;
        }

        private SmtpServerConfiguration GetSmtpConfiguration()
        {
            var configuration = _configuration.GetSection("SmtpServer")?.Get<SmtpServerConfiguration>();

            if (configuration == null)
                throw new EmailConfigurationException("Smtp configuration section was not found.");

            if (string.IsNullOrEmpty(configuration.FromEmail))
                throw new EmailConfigurationException($"{nameof(configuration.FromEmail)} is not configured.");

            if (configuration.SenderPeriod < 60)
                throw new EmailConfigurationException($"{configuration.SenderPeriod} must be grater then 60. ");

            return configuration;
        }

        private async void SendMessages(object state)
        {
            _logger.LogTrace("Sending messages.");
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var messages = await messageService.GetMessages();
                if (!messages.Any())
                {
                    _logger.LogTrace("No messages to send.");
                    return;
                }

                var successMessages = await emailService.SendBySmtp(messages, SmtpServerConfiguration);
                var failedMessage = messages.Except(successMessages).ToList();

                await messageService.DeleteMessages(successMessages);
                await messageService.AddFailedAttempt(failedMessage);
            }
            catch (Exception e)
            {
                _logger.LogDebug(default, e, "Sending messages failed.");
            }

            _logger.LogTrace("Messages sent.");
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
