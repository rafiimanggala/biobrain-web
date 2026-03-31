using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class ScheduledPaymentCourseConfiguration : IEntityTypeConfiguration<ScheduledPaymentCourseEntity>
    {
        public void Configure(EntityTypeBuilder<ScheduledPaymentCourseEntity> builder)
        {
            builder.HasKey(x => x.ScheduledPaymentCourseId);
            builder.HasOne(x => x.ScheduledPayment)
                .WithMany(x => x.ScheduledPaymentCourses)
                .HasForeignKey(x => x.ScheduledPaymentId);
        }
    }
}