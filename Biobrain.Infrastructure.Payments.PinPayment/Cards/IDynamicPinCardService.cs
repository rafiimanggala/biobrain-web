using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Cards
{
    public interface IDynamicPinCardService
    {
        Task<Card> PostCardAsync(Card card, string apiKey);
    }
}
