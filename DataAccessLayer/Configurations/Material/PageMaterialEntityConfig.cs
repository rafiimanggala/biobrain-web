using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Material
{
    public class PageMaterialEntityConfig : IEntityTypeConfiguration<PageMaterialEntity>
    {
        public void Configure(EntityTypeBuilder<PageMaterialEntity> b)
        {
            b.HasKey(_ => new {_.PageId, _.MaterialId});

            b.HasOne(x => x.Page)
	            .WithMany(x => x.PageMaterials)
	            .HasForeignKey(x => x.PageId);

            b.HasOne(x => x.Material)
	            .WithMany()
	            .HasForeignKey(x => x.MaterialId);
        }
    }
}
