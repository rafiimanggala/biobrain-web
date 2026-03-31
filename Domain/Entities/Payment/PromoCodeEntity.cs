using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;

namespace Biobrain.Domain.Entities.Payment
{
    /// <summary>
    /// Database entity of payment information
    /// </summary>
    public class PromoCodeEntity: ICreatedEntity, IUpdatedEntity
    {
        public Guid PromoCodeId { get; set; }

        public string Code { get; set; }
        public double? Amount { get; set; }
        public double? Percent { get; set; }

        public string? Country { get; set; }
        public PaymentPeriods? PaymentPeriod { get; set; }
        public int? BundleSize { get; set; }


        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}