namespace Biobrain.Application.Interfaces.Notifications
{
    public class FeedbackNotification : IEmailNotification
    {
        private readonly int _rating;
        private readonly string _feedback;
        private readonly string _userName;

        public FeedbackNotification(int rating, string feedback, string userName)
        {
            To = "support@biobrain.com.au";
            _rating = rating;
            _feedback = feedback;
            _userName = userName;
        }

        public string To { get; }
        public string Subject => $"BioBrain Feedback: {_rating}/5 stars from {_userName}";
        public string HtmlBody => @$"
<p><strong>User:</strong> {_userName}</p>
<p><strong>Rating:</strong> {_rating} / 5</p>
<p><strong>Feedback:</strong></p>
<p>{(_feedback.Length > 0 ? _feedback : "(No additional feedback provided)")}</p>
";
    }
}
