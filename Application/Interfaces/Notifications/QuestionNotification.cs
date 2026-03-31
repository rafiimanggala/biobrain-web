namespace Biobrain.Application.Interfaces.Notifications
{
    public record QuestionNotification : IEmailNotification
    {
        private readonly string _userName;
        private readonly string _email;
        private readonly string _question;
        private const string feedbackEmail = "feedback@biobrain.com.au";

        public QuestionNotification(string email, string userName, string question)
        {
            To = feedbackEmail;
            _email = email;
            _question = question;
            _userName = userName;
        }

        public string To { get; }
        public string Subject => "BioBrain question from user";
        public string HtmlBody => @$"
<p>User {_userName} ({_email}) asks a question:</p>

<p>{_question}</p>

<p>Kind regards,</p>
<p>The BioBrain Team</p>
<p>Need help with something?</p>
<p>Email us: <a href=""mailto:support@biobrain.com.au"">support@biobrain.com.au</a></p>
<p>Phone us: <a href=""tel:+61390171801"">+61 3 9017 1801</a></p>
";
    }
}