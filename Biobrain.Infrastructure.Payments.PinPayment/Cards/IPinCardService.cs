using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Cards
{
    public interface IPinCardService
    {
        Task<Card> PostCardAsync(Card card);
    }
}
