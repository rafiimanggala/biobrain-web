using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Vouchers
{
    public class VoucherEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid VoucherId { get; set; }
        public string Code { get; set; }
        public string Note { get; set; }
        public double TotalAmount { get; set; }
        public double AmountUsed { get; set; }
        public string Country { get; set; }
        public DateTime ExpiryDateUtc { get; set; }
        public DateTime RedeemExpiryDateUtc { get; set; }
        public DateTime? RedeemedDateUtc { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}