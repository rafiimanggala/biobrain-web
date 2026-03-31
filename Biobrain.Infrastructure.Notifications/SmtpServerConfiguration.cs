using JetBrains.Annotations;

namespace Biobrain.Infrastructure.Notifications
{
    [PublicAPI]
    public class SmtpServerConfiguration
    {
        public string Protocol { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public bool UseAuth { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public int SenderPeriod { get; set; }
        
    }
}