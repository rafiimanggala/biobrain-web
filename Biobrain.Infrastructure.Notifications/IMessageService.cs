using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Notifications;

namespace Biobrain.Infrastructure.Notifications
{
    internal interface IMessageService
    {
        Task SaveMessageAsync(string toEmail, string subject, string htmlBody);
        
        Task<List<EmailMessageEntity>> GetMessages();

        Task DeleteMessages(IEnumerable<EmailMessageEntity> messages);
        Task AddFailedAttempt(IEnumerable<EmailMessageEntity> messages);
    }
}