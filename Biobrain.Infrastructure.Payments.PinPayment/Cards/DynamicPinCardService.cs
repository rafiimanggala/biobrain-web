using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Extension;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Cards
{
    public class DynamicPinCardService : PinServiceBase, IDynamicPinCardService
    {
        public DynamicPinCardService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Card> PostCardAsync(Card card, string apiKey)
        {
            var response = await HttpClient
                .PostWithApiKey("1/cards", CreateBody(card), apiKey);

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }
    }
}
