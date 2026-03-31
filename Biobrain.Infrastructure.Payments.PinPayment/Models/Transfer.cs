using System;
using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class Transfer
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("total_debits")]
        public int TotalDebits { get; set; }
        [JsonProperty("total_credits")]
        public int TotalCredits { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("paid_at")]
        public DateTime PaidAt { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
        [JsonProperty("bank_account")]
        public BankAccount BankAccount { get; set; }
    }
}
