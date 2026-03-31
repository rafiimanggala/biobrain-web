using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Notifications;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Utils;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Biobrain.Infrastructure.Notifications
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger) => _logger = logger;

        public async Task<List<EmailMessageEntity>> SendBySmtp(List<EmailMessageEntity> messages, SmtpServerConfiguration smtpServerConfiguration)
        {
            _logger.LogInformation($" messages with ids: {messages.Aggregate(string.Empty, (acc, message) => $"{acc}, {message.EmailMessageId}")} ready to sent");

            using var client = new SmtpClient();
            try
            {
                var watch = Stopwatch.StartNew();

                await Connect(client, smtpServerConfiguration);
                if (!client.IsConnected)
                {
                    _logger.LogError("Smtp server not connected");
                    return new List<EmailMessageEntity>();
                }

                await Authenticate(client, smtpServerConfiguration);
                if (!client.IsAuthenticated)
                {
                    _logger.LogError("Smtp server not authenticated");
                    return new List<EmailMessageEntity>();
                }

                var sentMessages = await SentMessages(client, messages, smtpServerConfiguration);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"{sentMessages.Count} message(s) ready to delete");

                watch.Stop();
                _logger.LogTrace($"Performance - Send {sentMessages.Count} emails: {watch.ElapsedMilliseconds}");

                return sentMessages;
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, e.Message);
                return new List<EmailMessageEntity>();
            }
        }

        private async Task<List<EmailMessageEntity>> SentMessages(SmtpClient client, IEnumerable<EmailMessageEntity> messages, SmtpServerConfiguration smtpServerConfiguration)
        {
            var sentMessages = new List<EmailMessageEntity>();

            foreach (var message in messages)
            {
                if (await SendMessage(client, message, smtpServerConfiguration))
                    sentMessages.Add(message);
            }

            return sentMessages;
        }

        private async Task<bool> SendMessage(SmtpClient client, EmailMessageEntity message, SmtpServerConfiguration smtpServerConfiguration)
        {
            try
            {
                var watchMessageSend = Stopwatch.StartNew();

                _logger.LogInformation($"Attempt to send message with id {message.EmailMessageId}");

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(smtpServerConfiguration.FromName, smtpServerConfiguration.FromEmail));
                mimeMessage.To.Add(MailboxAddress.Parse(message.To));
                mimeMessage.Subject = message.Subject;
                mimeMessage.Body = await BuildMessageBody(message);

                await client.SendAsync(mimeMessage);

                _logger.LogInformation($"Message with id {message.EmailMessageId} successfully sent");

                watchMessageSend.Stop();
                _logger.LogTrace($"Performance - Message send: {watchMessageSend.ElapsedMilliseconds}");

                return true;
            }
            catch (SmtpCommandException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error sending message: {0}", ex.Message);
                sb.AppendFormat("\tStatusCode: {0}", ex.StatusCode);

                switch (ex.ErrorCode)
                {
                    case SmtpErrorCode.RecipientNotAccepted:
                        sb.AppendFormat("\tRecipient not accepted: {0}", ex.Mailbox);
                        break;
                    case SmtpErrorCode.SenderNotAccepted:
                        sb.AppendFormat("\tSender not accepted: {0}", ex.Mailbox);
                        break;
                    case SmtpErrorCode.MessageNotAccepted:
                        sb.Append("\tMessage not accepted.");
                        break;
                }

                message.FailReason = ex.Message;

                //if(ex.Message.Contains("No Such User Here"))
                //    _logger.LogTrace(default, ex, sb.ToString());
                //else
                    _logger.LogWarning(default, ex, sb.ToString());
            }
            catch (Exception ex)
            {
                message.FailReason = ex.Message;
                _logger.LogWarning(default, ex, null);
            }

            return false;
        }

        private static async Task<MimeEntity> BuildMessageBody(EmailMessageEntity message)
        {
            var builder = new BodyBuilder();

            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream("Biobrain.Infrastructure.Notifications.Assets.Three_Apps_Signature.png");
            var image = await builder.LinkedResources.AddAsync(@"Three_Apps_Signature.png", imageStream);
            image.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = $"{message.Body} <img src=\"cid:{image.ContentId}\">";
            return builder.ToMessageBody();
        }


        private async Task Connect(SmtpClient client, SmtpServerConfiguration smtpServerConfiguration)
        {
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
            try
            {
                var watchClientConnect = Stopwatch.StartNew();
                var secureSocketOptions = smtpServerConfiguration.Protocol.ToLower() switch
                {
                    "ssl" => SecureSocketOptions.SslOnConnect,
                    "tls" => SecureSocketOptions.StartTlsWhenAvailable,
                    _ => SecureSocketOptions.None
                };

                await client.ConnectAsync(smtpServerConfiguration.Host, smtpServerConfiguration.Port, secureSocketOptions);
                watchClientConnect.Stop();

                _logger.LogTrace($"Performance - Client connect: {watchClientConnect.ElapsedMilliseconds}");
            }
            catch (SmtpCommandException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error trying to connect: {0}", ex.Message);
                sb.AppendFormat("\tStatusCode: {0}", ex.StatusCode);

                _logger.LogError(default, ex, sb.ToString());

                throw;
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(default, ex, $"Protocol error while trying to connect: {ex.Message}");

                throw;
            }
        }

        private async Task Authenticate(SmtpClient client, SmtpServerConfiguration smtpServerConfiguration)
        {
            if (!smtpServerConfiguration.UseAuth)
                return;

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            try
            {
                var watchClientAuthenticate = Stopwatch.StartNew();
                await client.AuthenticateAsync(smtpServerConfiguration.Login, smtpServerConfiguration.Password);
                watchClientAuthenticate.Stop();

                _logger.LogInformation($"Performance - Client authenticate: {watchClientAuthenticate.ElapsedMilliseconds}");
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(default, ex, "Fail to authenticate");
                throw;
            }
            catch (SmtpCommandException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error trying to authenticate: {0}", ex.Message);
                sb.AppendFormat("\tStatusCode: {0}", ex.StatusCode);
                _logger.LogError(default, ex, sb.ToString());

                throw;
            }
            catch (SmtpProtocolException ex)
            {
                _logger.LogError(default, ex, $"Protocol error while trying to authenticate: {ex.Message}");

                throw;
            }
        }
    }
}
