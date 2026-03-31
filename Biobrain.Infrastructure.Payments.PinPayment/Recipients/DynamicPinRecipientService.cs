using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Extension;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Recipients
{
    public class DynamicPinRecipientService : PinServiceBase, IDynamicPinRecipientService
    {
        public DynamicPinRecipientService(PinPaymentsOptions options) 
            : base(options) { }

        public async Task<Recipient> GetRecipientAsync(string recipientToken, string apiKey)
        {
            var response = await HttpClient
                .GetWithApiKey("1/recipients" + $"/{recipientToken}", apiKey);

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }

        public async Task<Recipient> PostRecipientAsync(Recipient recipient, string apiKey)
        {
            var response = await HttpClient
                 .PostWithApiKey("1/recipients", CreateBody(recipient), apiKey);

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }

        public async Task<Recipient> PutRecipientAsync(Recipient recipient, string apiKey)
        {
            var response = await HttpClient
                .PutWithApiKey("1/recipients" + $"/{recipient.Token}", CreateBody(recipient), apiKey);

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }
    }
}
