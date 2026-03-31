using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class AdministratorChangedEmailNotification : IEmailNotification
    {
        private readonly Uri _loginUrl;

        public AdministratorChangedEmailNotification(string email, Uri loginUrl)
        {
            To = email;
            _loginUrl = loginUrl;
        }

        public string To { get; }
        public string Subject => "Email has been changed by administrator.";
        public string HtmlBody => @$"
<p>Your email has been changed by administrator.</p>
<p>You can login at <a href='{_loginUrl.AbsoluteUri}'>www.biobrain.com.au</a> using this email address as your username.</p>

<p>The BioBrain Team</p>
";
    }
}