using System;
using System.Text;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class AdministratorResetPasswordNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _resetPasswordUrl;

        public AdministratorResetPasswordNotification(string email, string userName, Uri resetPasswordUrl)
        {
            To = email;
            _userName = userName;
            _resetPasswordUrl = resetPasswordUrl;
        }

        public string To { get; }
        public string Subject => "Biobrain password reset by administrator";


        public string HtmlBody
        {
            get
            {
                var builder = new StringBuilder();
                builder.AddParagraph($"Dear {_userName}");
                builder.AddParagraph($"We're sending you this email because BioBrain administrator initiated a password reset. Click on this <a href='{_resetPasswordUrl.AbsoluteUri}'>link</a> to create a new password.");
                builder.AddParagraph("You can ignore this email and your password will not be changed.");
                builder.AddParagraph("The BioBrain Team");
                return builder.ToString();
            }
        }
    }
}