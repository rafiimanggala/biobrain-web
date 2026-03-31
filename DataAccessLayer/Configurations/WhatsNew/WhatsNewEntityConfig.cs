using Biobrain.Domain.Entities.WhatsNew;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.WhatsNew
{
    public class WhatsNewEntityConfig : IEntityTypeConfiguration<WhatsNewEntity>
    {
        public void Configure(EntityTypeBuilder<WhatsNewEntity> b)
        {
            b.HasKey(_ => _.WhatsNewId);

            b.Property(_ => _.Title)
             .IsRequired()
             .HasMaxLength(256);

            b.Property(_ => _.Content)
             .IsRequired();

            b.Property(_ => _.Version)
             .IsRequired()
             .HasMaxLength(32);

            b.Property(_ => _.PublishedAt)
             .IsRequired();

            b.Property(_ => _.IsActive)
             .HasDefaultValue(true);
        }
    }
}
