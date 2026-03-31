namespace Biobrain.Application.Interfaces.Notifications
{
    public class SubscriptionRenewNotification : IEmailNotification
    {
        private readonly string _firstName;


        public SubscriptionRenewNotification(string email, string firstName)
        {
            _firstName = firstName;
            To = email;
        }

        public string To { get; }
        public string Subject => $"Your BioBrain subscription is up for renewal in 7 days!"; 

        public string HtmlBody => @$"
<p>Hi {_firstName},</p>

<p>Your annual BioBrain subscription is about to renew! To keep accessing high-quality Biology, Chemistry, and Physics resources, you may to change your subscription for the upcoming year.</p> 

<ul>
<li>If you used a Year 11 product and now need Year 12 product, you must cancel your Year 11 subscription and purchase a Year 12 subscription.</li>

<li>If you’re in a two-year course like IB DP, your subscription will renew automatically—no action needed.</li>

<li>If you no longer need your BioBrain subscription, you can cancel it through the profile icon on the BioBrain platform. Otherwise your subscription will automatically be renewed.</li>
<ul>

<p>Don’t miss out! Renew now to keep learning without interruption.</p>

<p><a href=""https://go.biobrain.tech/login"">Renew Now</a></p>

<p>Stay curious,</p>
<p>The BioBrain Team</p>
<p>Need help with something?</p>
<p>Email us: <a href=""mailto:support@biobrain.com.au"">support@biobrain.com.au</a></p>
<p>Phone us: <a href=""tel:+61390171801"">+61 3 9017 1801</a></p>
";
    }
}