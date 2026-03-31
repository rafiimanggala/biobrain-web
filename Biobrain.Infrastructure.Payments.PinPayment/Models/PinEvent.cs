using System;
using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class PinEvent
    {
        [JsonProperty("event_token")]
        public string Token { get; set; }
        [JsonProperty("event_type")]
        public string Type { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }
}
