using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class StudentCreatedNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _loginUrl;
        private readonly Uri _resetPasswordUrl;

        public StudentCreatedNotification(string email, string userName, Uri loginUrl, Uri resetPasswordUrl)
        {
            To = email;
            _userName = userName;
            _loginUrl = loginUrl;
            _resetPasswordUrl = resetPasswordUrl;
        }

        public string To { get; }
        public string Subject => "BioBrain Login Details";
        public string HtmlBody => @$"
<p>Dear {_userName}</p>

<p>Welcome to BioBrain, leading STEM learning tools for Biology, Chemistry & Physics</p>
<p>You can login at <a href='{_loginUrl.AbsoluteUri}'>www.biobrain.com.au</a> using your email address as your username & your password as below.</p>
<p>Please click <a href='{_resetPasswordUrl.AbsoluteUri}'>here</a> and set your password.</p>

<p>The BioBrain Team</p>
";
    }
}