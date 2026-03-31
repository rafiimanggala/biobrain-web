using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class PinError
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string Description { get; set; }

        [JsonProperty("charge_token")]
        public string Token { get; set; }

        [JsonProperty("messages")]
        public ErrorMessage[] Messages { get; set; }
    }

    public class ErrorMessage
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("param")]
        public string Param { get; set; }

        [JsonProperty("charge")]
        public string[] Charge { get; set; }
    }
}
