namespace Biobrain.Application.Interfaces.Notifications
{
    public interface IEmailNotification
    {
        public string To { get; }
        public string Subject { get; }
        public string HtmlBody { get; }
    }
}