using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCodeEntity>
    {
        public void Configure(EntityTypeBuilder<PromoCodeEntity> builder)
        {
            builder.HasKey(x => x.PromoCodeId);
            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}