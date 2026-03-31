using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Material
{
    public class PageEntityConfig : IEntityTypeConfiguration<PageEntity>
    {
        public void Configure(EntityTypeBuilder<PageEntity> b)
        {
            b.HasKey(_ => _.PageId);

            b.HasMany(x => x.PageMaterials)
	            .WithOne()
	            .HasForeignKey(x => x.PageId);

            b.HasOne(x => x.ContentTreeNode)
	            .WithMany()
	            .HasForeignKey(x => x.ContentTreeId);
        }
    }
}
