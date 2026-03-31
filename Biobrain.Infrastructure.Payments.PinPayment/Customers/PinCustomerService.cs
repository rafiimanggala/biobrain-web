using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Customers
{
    public class PinCustomerService : PinServiceBase, IPinCustomerService
    {
        public PinCustomerService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Customer> GetCustomerAsync(string customerToken)
        {
            var response = await HttpClient
                .GetAsync("1/customers" + $"/{customerToken}");

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }

        public async Task<IEnumerable<Card>> GetCustomerCardsAsync(string customerToken)
        {
            var response = await HttpClient
               .GetAsync("1/customers" + $"/{customerToken}" + "/cards");

            return await ResponseProcessingWithBodyAsync<IEnumerable<Card>>(response);
        }

        public async Task<Customer> PostCustomerAsync(Customer customer)
        {
            var response = await HttpClient
                .PostAsync("1/customers", CreateBody(customer));

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }

        public async Task<Card> PostNewCardToCustomerAsync(string customerToken, Card card)
        {
            var response = await HttpClient
              .PostAsync("1/customers" + $"/{customerToken}" + "/cards", CreateBody(card));

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }

        public async Task<Card> PostNewCardToCustomerAsync(string customerToken, CardTokenModel card)
        {
            var response = await HttpClient
              .PostAsync("1/customers" + $"/{customerToken}" + "/cards", CreateBody(card));

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }

        public async Task RemoveNotDefaultCustomerCardAsync(string customerToken, string cardToken)
        {
            var response = await HttpClient
                .DeleteAsync("1/customers" + $"/{customerToken}" + "/cards" + $"/{cardToken}");

            await ResponseProcessingAsync(response);
        }

        public async Task<Customer> SetCardAsDefaultAsync(string customerToken, string cardToken)
        {
            var response = await HttpClient
               .PutAsync("1/customers" + $"/{customerToken}", CreateBody(new Customer
               {
                   PrimaryCardToken = cardToken
               }));

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }
    }
}
