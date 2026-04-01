using Microsoft.Maui.ApplicationModel;

namespace BioBrain.Interfaces
{
    public interface IEmailSender
    {
        void Compose(EmailMessage message, string textMessage);
    }
}