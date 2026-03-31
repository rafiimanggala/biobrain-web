using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class LastPaidScheduledPaymentConfiguration : IEntityTypeConfiguration<LastPaidScheduledPaymentEntity>
    {
        public void Configure(EntityTypeBuilder<LastPaidScheduledPaymentEntity> builder)
        {
            builder.HasKey(x => x.LastPaidScheduledPaymentId);

            builder.HasOne(x => x.ScheduledPayment)
                .WithMany()
                .HasForeignKey(x => x.ScheduledPaymentId);
        }
    }
}