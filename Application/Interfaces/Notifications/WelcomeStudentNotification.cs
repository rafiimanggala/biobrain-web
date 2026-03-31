using System;

namespace Biobrain.Application.Interfaces.Notifications
{
    public class WelcomeStudentNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly Uri _link;

        public WelcomeStudentNotification(string email, string userName, Uri link)
        {
            To = email;
            _userName = userName;
            _link = link;
        }

        public string To { get; }
        public string Subject => $"Welcome to BioBrain!";
        public string HtmlBody => @$"
<p>Hello {_userName},</p>

<pGreat news: you’re signed up to use BioBrain! Now you’ll be able to learn, revise and track your way through key concepts in your course, anywhere, anytime with our webapp. We'll cover content that you need to know, helping you to identify your strengths and address your weaknesses.</p>
<p><a href=""{_link.AbsoluteUri}"">Log in here</a> and start learning and revising using the detailed learning material and quiz questions. To get the most out of BioBrain, we recommend:</p>
<ul>
<li>Trying to complete at least <b>one quiz every day</b> to reinforce concepts covered in class or at home.</li>
<li>Track your progress using the <b>Results</b> tab.</li>
<li>Learn key terms with the illustrated <b>glossary</b>.</li>
</ul>
<p>BioBrain has everything you need in one online platform: learning material, thousands of quiz questions and results to track your progress. As you work through BioBrain, the platform will automatically track your areas of strength and weakness, colour coding the concepts to show you where you need to focus your revision!</p>
<p>As BioBrain is a webapp, it can be saved on your mobile device, allowing you to study and revise anytime, anywhere.</p>
<p>If you need any help or would like to give us any feedback on how to make BioBrain even better, please don’t hesitate to contact support by emailing: <a href""mailto:support@biobrain.com.au"">support@biobrain.com.au</a>.</p>
<p>Happy learning,</p>
<p>The BioBrain Team</p>
";
    }
}