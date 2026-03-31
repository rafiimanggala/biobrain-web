using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Notifications;

namespace Biobrain.Infrastructure.Notifications
{
    public interface IEmailService
    {
        Task<List<EmailMessageEntity>> SendBySmtp(List<EmailMessageEntity> messages, SmtpServerConfiguration smtpServerConfiguration);
    }
}