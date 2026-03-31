namespace Biobrain.Infrastructure.Payments.PinPayments
{
    public class PinPaymentsOptions
    {
        public PinPaymentsOptions(string apiKey, string baseUrl)
        {
            ApiKey = apiKey;
            BaseUrl = baseUrl;
        }

        public string ApiKey { get; }
        public string BaseUrl { get; }
    }
}