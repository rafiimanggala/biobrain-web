using System;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.Notifications;

namespace Biobrain.Infrastructure.Notifications
{
    internal class NotificationService : INotificationService
    {
        private readonly IMessageService _messageService;

        public NotificationService(IMessageService messageService) => _messageService = messageService;

        public async Task Send<T>(T notification) where T : IEmailNotification
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            await _messageService.SaveMessageAsync(notification.To, notification.Subject, notification.HtmlBody);
        }
    }
}
