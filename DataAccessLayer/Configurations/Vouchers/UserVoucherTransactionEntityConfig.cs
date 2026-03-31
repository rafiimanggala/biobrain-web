using Biobrain.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Vouchers
{
    public class UserVoucherTransactionEntityConfig : IEntityTypeConfiguration<UserVoucherTransactionEntity>
    {
        public void Configure(EntityTypeBuilder<UserVoucherTransactionEntity> b)
        {
            b.HasKey(_ => _.UserVoucherTransactionId);

            b.HasOne(_ => _.UserVoucher).WithMany().HasForeignKey(_ => _.UserVoucherId).HasPrincipalKey(_ => _.UserVoucherId);
        }
    }
}
