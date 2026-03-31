using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class ScheduledPaymentConfiguration : IEntityTypeConfiguration<ScheduledPaymentEntity>
    {
        public void Configure(EntityTypeBuilder<ScheduledPaymentEntity> builder)
        {
            builder.HasKey(x => x.ScheduledPaymentId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.ScheduledPayments)
                .HasForeignKey(x => x.UserId);
        }
    }
}