using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Customers
{
    public interface IPinCustomerService
    {
        Task<Customer> PostCustomerAsync(Customer customer);
        Task<Customer> GetCustomerAsync(string customerToken);

        Task<Card> PostNewCardToCustomerAsync(string customerToken, Card card);
        Task<Card> PostNewCardToCustomerAsync(string customerToken, CardTokenModel card);
        Task RemoveNotDefaultCustomerCardAsync(string customerToken, string cardToken);

        Task<Customer> SetCardAsDefaultAsync(string customerToken, string cardToken);
        Task<IEnumerable<Card>> GetCustomerCardsAsync(string customerToken);
    }
}
