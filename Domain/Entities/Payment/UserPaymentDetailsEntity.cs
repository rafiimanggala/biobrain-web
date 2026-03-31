using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Payment
{
    public class UserPaymentDetailsEntity: ICreatedEntity, IUpdatedEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid UserPaymentId { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public PaymentMethods PaymentMethod { get; set; }
        public string PinPaymentCustomerRefId { get; set; }
        public string IpAddress { get; set; }
    }
}