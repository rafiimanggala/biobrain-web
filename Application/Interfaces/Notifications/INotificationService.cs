using System.Threading.Tasks;

namespace Biobrain.Application.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task Send<T>(T notification) where T : IEmailNotification;
    }
}
