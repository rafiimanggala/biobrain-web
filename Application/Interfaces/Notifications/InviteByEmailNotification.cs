using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class InviteByEmailNotification : IEmailNotification
    {
        private readonly string _teacherName;
        private readonly string _className;
        private readonly string _classCode;
        private readonly Uri _link;

        public InviteByEmailNotification(string email, string teacherName, string className, string classCode, Uri link)
        {
            To = email;
            _teacherName = teacherName;
            _link = link;
            _className = className;
            _classCode = classCode;
        }

        public string To { get; }
        public string Subject => $"Invite To BioBrain By {_teacherName}";
        public string HtmlBody => @$"
<p>We're sending you this email because {_teacherName} invites you to Sign Up in BioBrain and join the class {_className}.</p>
<p>Click on this <a href='{_link.AbsoluteUri}'>link</a> an create an account. Use <b>{_classCode}</b> to join the class.</p>

<p>The BioBrain Team</p>
";
    }
}