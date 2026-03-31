namespace Biobrain.Application.Interfaces.Notifications
{
    public record PaymentErrorAdminNotification : IEmailNotification
    {
	    private readonly string _message;
	    private readonly string _details;

	    public PaymentErrorAdminNotification(string email, string message, string details)
        {
            To = email;
            _message = message;
            _details = details;
        }

        public string To { get; }
        public string Subject => "Biobrain Payment Error";
        public string HtmlBody => @$"
<p>Dear admin</p>

<p>Error occurred during payment process.</p>
<p>Message: {_message}</p>
<p>Details: {_details}</p>

<p>Kind regards,</p>
<p>The BioBrain Team</p>
";
    }
}