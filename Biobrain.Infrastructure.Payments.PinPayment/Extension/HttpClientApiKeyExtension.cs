using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Biobrain.Infrastructure.Payments.PinPayments.Extension
{
    internal static class HttpClientApiKeyExtension
    {
        public static Task<HttpResponseMessage> GetWithApiKey(this HttpClient client, string requestUri, string apiKey)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey + ":")));
            return client.GetAsync(requestUri);
        }

        public static Task<HttpResponseMessage> PostWithApiKey(this HttpClient client, string requestUri, HttpContent content, string apiKey)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey + ":")));
            return client.PostAsync(requestUri, content);
        }

        public static Task<HttpResponseMessage> PutWithApiKey(this HttpClient client, string requestUri, HttpContent content, string apiKey)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey + ":")));
            return client.PutAsync(requestUri, content);
        }

        public static Task<HttpResponseMessage> DeleteWithApiKey(this HttpClient client, string requestUri, string apiKey)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes(apiKey + ":")));
            return client.DeleteAsync(requestUri);
        }
    }
}