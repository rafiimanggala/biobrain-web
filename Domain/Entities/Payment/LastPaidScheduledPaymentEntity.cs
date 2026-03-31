using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Payment
{
    public class LastPaidScheduledPaymentEntity : ICreatedEntity, IUpdatedEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid LastPaidScheduledPaymentId { get; set; }

        public Guid ScheduledPaymentId { get; set; }
        public ScheduledPaymentEntity ScheduledPayment { get; set; }
    }
}