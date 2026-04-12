using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
    public class ContentImageEntityConfig : IEntityTypeConfiguration<ContentImageEntity>
    {
        public void Configure(EntityTypeBuilder<ContentImageEntity> b)
        {
            b.HasKey(_ => _.ImageId);
            b.Property(_ => _.Code).HasMaxLength(100);
            b.Property(_ => _.FileName).HasMaxLength(500);
            b.Property(_ => _.Description).HasMaxLength(1000);
            b.Property(_ => _.ContentType).HasMaxLength(100);
            b.HasIndex(_ => _.Code);
        }
    }
}
