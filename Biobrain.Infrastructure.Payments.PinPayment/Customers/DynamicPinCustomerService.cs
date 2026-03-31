using System.Collections.Generic;
using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Extension;
using Biobrain.Infrastructure.Payments.PinPayments.Models;

namespace Biobrain.Infrastructure.Payments.PinPayments.Customers
{
    public class DynamicPinCustomerService : PinServiceBase, IDynamicPinCustomerService
    {
        public DynamicPinCustomerService(PinPaymentsOptions options)
            : base(options) { }

        public async Task<Customer> GetCustomerAsync(string customerToken, string apiKey)
        {
            var response = await HttpClient
                .GetWithApiKey("1/customers" + $"/{customerToken}", apiKey);

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }

        public async Task<IEnumerable<Card>> GetCustomerCardsAsync(string customerToken, string apiKey)
        {
            var response = await HttpClient
               .GetWithApiKey("1/customers" + $"/{customerToken}" + "/cards", apiKey);

            return await ResponseProcessingWithBodyAsync<IEnumerable<Card>>(response);
        }

        public async Task<Customer> PostCustomerAsync(Customer customer, string apiKey)
        {
            var response = await HttpClient
                .PostWithApiKey("1/customers", CreateBody(customer), apiKey);

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }

        public async Task<Card> PostNewCardToCustomerAsync(string customerToken, Card card, string apiKey)
        {
            var response = await HttpClient
              .PostWithApiKey("1/customers" + $"/{customerToken}" + "/cards", CreateBody(card), apiKey);

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }

        public async Task<Card> PostNewCardToCustomerAsync(string customerToken, CardTokenModel card, string apiKey)
        {
            var response = await HttpClient
              .PostWithApiKey("1/customers" + $"/{customerToken}" + "/cards", CreateBody(card), apiKey);

            return await ResponseProcessingWithBodyAsync<Card>(response);
        }

        public async Task RemoveNotDefaultCustomerCardAsync(string customerToken, string cardToken, string apiKey)
        {
            var response = await HttpClient
                .DeleteWithApiKey("1/customers" + $"/{customerToken}" + "/cards" + $"/{cardToken}", apiKey);

            await ResponseProcessingAsync(response);
        }

        public async Task<Customer> SetCardAsDefaultAsync(string customerToken, string cardToken, string email, string apiKey)
        {
            var response = await HttpClient
               .PutWithApiKey("1/customers" + $"/{customerToken}", CreateBody(new Customer
               {
                   PrimaryCardToken = cardToken,
                   Email = email
               }), apiKey);

            return await ResponseProcessingWithBodyAsync<Customer>(response);
        }
    }
}
