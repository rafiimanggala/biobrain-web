using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Cards
{
    public class PinCardService : PinServiceBase, IPinCardService
    {
        public PinCardService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Card> PostCardAsync(Card card)
        {
            var response = await HttpClient
                .PostAsync("1/cards", CreateBody(card));

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }
    }
}
