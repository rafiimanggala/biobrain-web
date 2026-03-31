using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Biobrain.Infrastructure.Payments.PinPayments.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PinPayments;

namespace Biobrain.Infrastructure.Payments.PinPayments
{
    public abstract class PinServiceBase
    {
        protected HttpClient HttpClient { get; set; }

        protected PinPaymentsOptions Options;

        protected PinServiceBase(PinPaymentsOptions options)
        {
            Options = options;

            HttpClient = GetClient();
        }

        HttpClient GetClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.BaseAddress = new Uri(Options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(100);
            return client;
        }

        protected StringContent CreateBody<T>(T model)
            => new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

        protected async Task<T> ReadBodyAsync<T>(HttpContent content)
        {
            var @string = await content
                .ReadAsStringAsync();
            JObject jObject = JObject.Parse(@string);
            return JsonConvert
                .DeserializeObject<T>(jObject.SelectToken("response").ToString());
        }

        protected async Task<T> ResponseProcessingWithBodyAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await ReadBodyAsync<T>(response.Content);

            var error = await ReadErrorBodyAsync(response.Content);
            throw new PinException(response.StatusCode, error, error.Description);
        }

        protected async Task ResponseProcessingAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await ReadErrorBodyAsync(response.Content);
                throw new PinException(response.StatusCode, error, error.Description);
            }
        }

        protected async Task<PinError> ReadErrorBodyAsync(HttpContent content)
        {
            var @string = await content
                .ReadAsStringAsync();
            return JsonConvert
              .DeserializeObject<PinError>(@string);
        }
    }
}
