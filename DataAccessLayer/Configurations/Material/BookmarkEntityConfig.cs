using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Material
{
    public class BookmarkEntityConfig : IEntityTypeConfiguration<BookmarkEntity>
    {
        public void Configure(EntityTypeBuilder<BookmarkEntity> b)
        {
            b.HasKey(_ => _.BookmarkId);
            b.HasOne(_ => _.Material).WithMany().HasForeignKey(x => x.MaterialId);
            b.HasOne(_ => _.User).WithMany().HasForeignKey(x => x.UserId);
            b.HasOne(_ => _.Course).WithMany().HasForeignKey(x => x.CourseId);
        }
    }
}
