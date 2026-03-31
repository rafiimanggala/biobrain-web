using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class Customer
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("card_token")]
        public string CardToken { get; set; }

        [JsonProperty("primary_card_token")]
        public string PrimaryCardToken { get; set; }

        [JsonProperty("card")]
        public Card Card { get; set; }
    }
}
