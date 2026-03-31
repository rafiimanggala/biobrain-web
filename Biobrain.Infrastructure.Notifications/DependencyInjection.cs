using Biobrain.Application.Interfaces.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biobrain.Infrastructure.Notifications
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddHostedService<MessageSenderService>();

            return services;
        }
    }
}
