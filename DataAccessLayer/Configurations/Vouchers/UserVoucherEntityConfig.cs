using Biobrain.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Vouchers
{
    public class UserVoucherEntityConfig : IEntityTypeConfiguration<UserVoucherEntity>
    {
        public void Configure(EntityTypeBuilder<UserVoucherEntity> b)
        {
            b.HasKey(_ => _.UserVoucherId);

            b.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id);
            b.HasOne(_ => _.Voucher).WithMany().HasForeignKey(_ => _.VoucherId).HasPrincipalKey(_ => _.VoucherId);
        }
    }
}
