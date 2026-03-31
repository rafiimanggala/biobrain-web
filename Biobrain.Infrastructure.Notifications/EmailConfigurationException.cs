using System;

namespace Biobrain.Infrastructure.Notifications
{
    public class EmailConfigurationException : Exception
    {
        public EmailConfigurationException(string message) : base(message)
        {
        }
    }
}