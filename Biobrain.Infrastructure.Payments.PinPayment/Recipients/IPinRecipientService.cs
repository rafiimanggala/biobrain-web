using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Recipients
{
    public interface IPinRecipientService
    {
        Task<Recipient> PostRecipientAsync(Recipient recipient);
        Task<Recipient> GetRecipientAsync(string recipientToken);
        Task<Recipient> PutRecipientAsync(Recipient recipient);
    }
}
