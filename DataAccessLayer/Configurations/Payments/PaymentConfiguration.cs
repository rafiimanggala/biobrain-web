using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.HasKey(x => x.PaymentId);
            builder.HasOne(x => x.ScheduledPayment)
                .WithMany()
                .HasForeignKey(x => x.ScheduledPaymentId);
        }
    }
}