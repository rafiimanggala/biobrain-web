using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class FreeTrialWelcomeNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _link;

        public FreeTrialWelcomeNotification(string email, string userName, Uri link)
        {
            To = email;
            _userName = userName;
            _link = link;
        }

        public string To { get; }
        public string Subject => $"Welcome to BioBrain!";
        public string HtmlBody => @$"
<p>Hello {_userName},</p>

<p>Thank you for signing up to use BioBrain! Now you will be able to learn, revise, and track your way through key concepts in your course, anywhere, anytime with our webapp.</p>
<p><a href=""{_link.AbsoluteUri}"">Log in here</a> to start learning and revising using our detailed learning materials and quiz questions. To get the most out of BioBrain, we recommend:</p>
<ol>
<li>Completing at least one quiz every day to reinforce concepts covered in class or at home.</li>
<li>Learning key terms with the illustrated Glossary. </li>
<li>Tracking your progress using the Results tab.</li>
</ol>
<p>BioBrain has everything you need in one online platform: learning materials, thousands of quiz questions, and results to track your progress. As you work through BioBrain, the platform automatically tracks your areas of strength and weakness, colour coding the concepts to show you where you need to focus your revision!</p>
<p>As BioBrain is a webapp, it can be saved on your mobile device, allowing you to study and revise anywhere, anytime. <p>
<p>If you need any help or would like to give us feedback on how to make BioBrain even better, please don’t hesitate to contact support by emailing: <a href""mailto:support@biobrain.com.au"">support@biobrain.com.au</a>.</p>
<p>Happy learning,</p>
<p>The BioBrain Team</p>
";
    }
}