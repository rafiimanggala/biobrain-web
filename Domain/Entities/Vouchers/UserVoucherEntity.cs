using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.SiteIdentity;

namespace Biobrain.Domain.Entities.Vouchers
{
    public class UserVoucherEntity: ICreatedEntity, IUpdatedEntity
    {
        public Guid UserVoucherId { get; set; }
        public string SchoolName { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        public Guid VoucherId { get; set; }
        public VoucherEntity Voucher { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}