using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Customers
{
    public interface IDynamicPinCustomerService
    {
        Task<Customer> PostCustomerAsync(Customer customer, string apiKey);
        Task<Customer> GetCustomerAsync(string customerToken, string apiKey);

        Task<Card> PostNewCardToCustomerAsync(string customerToken, Card card, string apiKey);
        Task<Card> PostNewCardToCustomerAsync(string customerToken, CardTokenModel card, string apiKey);
        Task RemoveNotDefaultCustomerCardAsync(string customerToken, string cardToken, string apiKey);

        Task<Customer> SetCardAsDefaultAsync(string customerToken, string cardToken, string email, string apiKey);
        Task<IEnumerable<Card>> GetCustomerCardsAsync(string customerToken, string apiKey);
    }
}
