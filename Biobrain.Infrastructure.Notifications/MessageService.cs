using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Infrastructure.Notifications
{
    internal class MessageService : IMessageService
    {
        private readonly IDb _db;
        private readonly ILogger<MessageService> _logger;

        public MessageService(ILogger<MessageService> logger, IDb db)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<List<EmailMessageEntity>> GetMessages()
        {
            return await _db.EmailMessageQueue.AsNoTracking().Where(_ => !_.IsDisabled).ToListAsync();
        }

        public async Task SaveMessageAsync(string toEmail, string subject, string htmlBody)
        {
            await _db.EmailMessageQueue.AddAsync(new EmailMessageEntity
                                             {
                                                 Attempts = 0,
                                                 Body = htmlBody,
                                                 Subject = subject,
                                                 To = toEmail,
                                             });
            await _db.SaveChangesAsync();
        }
        
        public async Task DeleteMessages(IEnumerable<EmailMessageEntity> messages)
        {
            foreach (var message in messages)
            {
                _db.EmailMessageQueue.Remove(message);
                _logger.LogTrace($"Delete message: {message.EmailMessageId}");
            }

            await _db.SaveChangesAsync();
        }

        public async Task AddFailedAttempt(IEnumerable<EmailMessageEntity> messages)
        {
            foreach (var message in messages)
            {
                message.Attempts += 1;
                if(message.Attempts > 5)
                    message.IsDisabled = true;
                _db.EmailMessageQueue.Update(message);
                _logger.LogTrace($"Update message: {message.EmailMessageId}");
            }

            await _db.SaveChangesAsync();
        }
    }
}
