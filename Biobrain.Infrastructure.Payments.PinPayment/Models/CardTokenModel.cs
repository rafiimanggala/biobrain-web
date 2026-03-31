using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class CardTokenModel
    {
        [JsonProperty("card_token")]
        public string CardToken { get; set; }
    }
}