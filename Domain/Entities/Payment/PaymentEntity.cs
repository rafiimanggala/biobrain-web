using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;

namespace Biobrain.Domain.Entities.Payment
{
    /// <summary>
    /// Database entity of payment information
    /// </summary>
    public class PaymentEntity: ICreatedEntity, IUpdatedEntity
    {
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Payment entity id
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// ScheduledPaymentEntity -> ScheduledPaymentId
        /// </summary>
        public Guid ScheduledPaymentId { get; set; }

        /// <summary>
        /// Scheduled payment
        /// </summary>
        public ScheduledPaymentEntity ScheduledPayment { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Product Description
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public PaymentStatusEnum Status { get; set; }

        /// <summary>
        /// Payment system ref id of charge from customer to system account
        /// </summary>
        public string ChargeRefId { get; set; }


        /// <summary>
        /// Payment system ref id of transfer from system account to recipient
        /// </summary>
        public string TransferRefId { get; set; }

        /// <summary>
        /// Json string of payment system answer
        /// </summary>
        public string FailedPayload { get; set; }
    }
}