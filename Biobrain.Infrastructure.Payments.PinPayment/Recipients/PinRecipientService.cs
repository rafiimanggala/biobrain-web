using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Recipients
{
    public class PinRecipientService : PinServiceBase, IPinRecipientService
    {
        public PinRecipientService(PinPaymentsOptions options) 
            : base(options) { }

        public async Task<Recipient> GetRecipientAsync(string recipientToken)
        {
            var response = await HttpClient
                .GetAsync("1/recipients" + $"/{recipientToken}");

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }

        public async Task<Recipient> PostRecipientAsync(Recipient recipient)
        {
            var response = await HttpClient
                 .PostAsync("1/recipients", CreateBody(recipient));

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }

        public async Task<Recipient> PutRecipientAsync(Recipient recipient)
        {
            var response = await HttpClient
                .PutAsync("1/recipients" + $"/{recipient.Token}", CreateBody(recipient));

            return await ResponseProcessingWithBodyAsync<Recipient>(response);
        }
    }
}
