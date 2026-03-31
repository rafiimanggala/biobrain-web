using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class BankAccount
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("bsb")]
        public string Bsb { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }
        [JsonProperty("branch")]
        public string Branch { get; set; }
    }
}
