using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Payment
{
    /// <summary>
    /// Database entity of payment information
    /// </summary>
    public class UserPromoCodeEntity: ICreatedEntity, IUpdatedEntity
    {
        public Guid UserPromoCodeId { get; set; }
        public Guid PromoCodeId { get; set; }
        public Guid UserId { get; set; }

        public PromoCodeEntity PromoCode { get; set; }
        public UserEntity User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}