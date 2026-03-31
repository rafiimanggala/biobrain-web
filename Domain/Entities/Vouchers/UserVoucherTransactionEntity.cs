using System;
using Biobrain.Domain.Base;

namespace Biobrain.Domain.Entities.Vouchers
{
    public class UserVoucherTransactionEntity : ICreatedEntity, IUpdatedEntity
    {
        public Guid UserVoucherTransactionId { get; set; }

        public double Amount { get; set; }

        public Guid UserVoucherId { get; set; }
        public UserVoucherEntity UserVoucher { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}