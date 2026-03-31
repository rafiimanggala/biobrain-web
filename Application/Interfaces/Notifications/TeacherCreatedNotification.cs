using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public record TeacherCreatedNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _loginUrl;
        private readonly Uri _resetPasswordUrl;

        public TeacherCreatedNotification(string email, string userName, Uri loginUrl, Uri resetPasswordUrl)
        {
            To = email;
            _resetPasswordUrl = resetPasswordUrl;
            _userName = userName;
            _loginUrl = loginUrl;
        }

        public string To { get; }
        public string Subject => "BioBrain Login Details";
        public string HtmlBody => @$"
<p>Dear {_userName}</p>

<p>Welcome to BioBrain, a leading learning platform for Biology, Chemistry and Physics teachers and their students.</p>
<p>To set your password please click <a href='{_resetPasswordUrl.AbsoluteUri}'>here</a>.</p>
<p>Once you have set your password, please use your email address as your username and the password you just set.</p>
<p>Once you have set up your password, you can login at <a href='{_loginUrl.AbsoluteUri}'>www.biobrain.com.au</a></p>

<p>Kind regards,</p>
<p>The BioBrain Team</p>
<p>Need help with something?</p>
<p>Email us: <a href=""mailto:support@biobrain.com.au"">support@biobrain.com.au</a></p>
<p>Phone us: <a href=""tel:+61390171801"">+61 3 9017 1801</a></p>
";
    }
}