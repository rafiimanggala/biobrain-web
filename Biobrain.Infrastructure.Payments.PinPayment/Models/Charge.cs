using System;
using Newtonsoft.Json;

namespace Biobrain.Infrastructure.Payments.PinPayments.Models
{
    public class Charge
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("capture")]
        public bool Capture { get; set; }

        [JsonProperty("customer_token")]
        public string CustomerToken { get; set; }

        [JsonProperty("card_token")]
        public string CardToken { get; set; }

        [JsonProperty("card")]
        public Card Card { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("status_message")]
        public string StatusMessage { get; set; }
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("amount_refunded")]
        public int AmountRefunded { get; set; }
        [JsonProperty("total_fees")]
        public int? TotalFees { get; set; }
        [JsonProperty("merchant_entitlement")]
        public int? MerchantEntitlement { get; set; }
        [JsonProperty("refund_pending")]
        public bool RefundPending { get; set; }
        [JsonProperty("authorisation_expired")]
        public bool AuthorisationExpired { get; set; }
        [JsonProperty("captured")]
        public bool Captured { get; set; }
        [JsonProperty("settlement_currency")]
        public string SettlementCurrency { get; set; }
    }
}
