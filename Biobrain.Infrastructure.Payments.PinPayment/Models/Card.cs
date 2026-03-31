using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class Card
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("customer_token")]
        public string CustomerToken { get; set; }

        [JsonProperty("scheme")]
        public string Scheme { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("display_number")]
        public string DisplayNumber { get; set; }
        [JsonProperty("expiry_month")]
        public int ExpiryMonth { get; set; }
        [JsonProperty("expiry_year")]
        public int ExpiryYear { get; set; }
        [JsonProperty("cvc")]
        public string Cvc { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("primary")]
        public bool? Primary { get; set; }

        [JsonProperty("address_line1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("address_line2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("address_city")]
        public string AddressCity { get; set; }

        [JsonProperty("address_postcode")]
        public string AddressPostcode { get; set; }
        [JsonProperty("address_state")]
        public string AddressState { get; set; }
        [JsonProperty("address_country")]
        public string AddressCountry { get; set; }
    }
}
