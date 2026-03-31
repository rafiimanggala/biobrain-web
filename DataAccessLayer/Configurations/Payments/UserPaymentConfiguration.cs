using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class UserPaymentConfiguration : IEntityTypeConfiguration<UserPaymentDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<UserPaymentDetailsEntity> builder)
        {
            builder.HasKey(x => x.UserPaymentId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.PaymentDetails)
                .HasForeignKey(x => x.UserId);
        }
    }
}