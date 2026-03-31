using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Recipients
{
    public interface IDynamicPinRecipientService
    {
        Task<Recipient> PostRecipientAsync(Recipient recipient, string apiKey);
        Task<Recipient> GetRecipientAsync(string recipientToken, string apiKey);
        Task<Recipient> PutRecipientAsync(Recipient recipient, string apiKey);
    }
}
