namespace Biobrain.Application.Interfaces.Notifications
{
    public class EmailClassNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly string _teacherName;
        private readonly string _text;
        private readonly string _className;
        private readonly string _icon;

        public EmailClassNotification(string email, string userName, string text, string teacherName, string className, string icon)
        {
            To = email;
            _text = text;
            _userName = userName;
            _teacherName = teacherName;
            _className = className;
            _icon = icon;
        }

        public string To { get; }
        public string Subject => $"BioBrain: {_className} {_icon}";
        public string HtmlBody => @$"
<p>Dear {_userName},</p>

<p>{_text}</p>

<p>Kind regards,</p>
<p>{_teacherName}</p>
";
    }
}