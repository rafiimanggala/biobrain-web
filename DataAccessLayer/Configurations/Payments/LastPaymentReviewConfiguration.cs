using Biobrain.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Payments
{
    public class LastPaymentReviewConfiguration : IEntityTypeConfiguration<LastPaymentReviewEntity>
    {
        public void Configure(EntityTypeBuilder<LastPaymentReviewEntity> builder)
        {
            builder.HasKey(x => x.LastPaymentReviewId);
        }
    }
}