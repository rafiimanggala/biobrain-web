using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class UserPromoCodeConfiguration : IEntityTypeConfiguration<UserPromoCodeEntity>
    {
        public void Configure(EntityTypeBuilder<UserPromoCodeEntity> builder)
        {
            builder.HasKey(x => x.UserPromoCodeId);
            builder.HasOne(_ => _.PromoCode).WithMany().HasForeignKey(_ => _.PromoCodeId);
            builder.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId);
        }
    }
}