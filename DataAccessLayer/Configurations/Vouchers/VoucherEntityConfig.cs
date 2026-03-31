using Biobrain.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Vouchers
{
    public class VoucherEntityConfig : IEntityTypeConfiguration<VoucherEntity>
    {
        public void Configure(EntityTypeBuilder<VoucherEntity> b)
        {
            b.HasKey(_ => _.VoucherId);
            b.HasIndex(_ => new {_.Code}).IsUnique();
        }
    }
}
