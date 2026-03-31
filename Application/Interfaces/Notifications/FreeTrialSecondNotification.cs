using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class FreeTrialSecondNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _link;

        public FreeTrialSecondNotification(string email, string userName, Uri link)
        {
            To = email;
            _userName = userName;
            _link = link;
        }

        public string To { get; }
        public string Subject => $"BioBrain – helping you get better exam results!";
        public string HtmlBody => @$"
<p>Hello {_userName},</p>

<p>We hope you are enjoying using BioBrain. I wanted to reach out and see if I could answer any questions, so that you could make the most of your 14-day trial.</p>
<p>Unlike typical revision guides, the BioBrain platform has far more content, hundreds of practice questions, an illustrated glossary and automated tracking allowing you to see immediately the impact it is having on your results!!</p>
<p>Sign in to <a href=""{_link.AbsoluteUri}"">BioBrain</a> to continue your journey to better results.Please let me know if you have any questions. Email <a href""mailto:support@biobrain.com.au"">support@biobrain.com.au</a></p>
<p>I look forward to hearing from you.</p>
<p>Happy learning,</p>
<p>Caroline, BioBrain Founder</p>
";
    }
}