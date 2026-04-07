using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Material
{
    public class ExcludedMaterialEntityConfig : IEntityTypeConfiguration<ExcludedMaterialEntity>
    {
        public void Configure(EntityTypeBuilder<ExcludedMaterialEntity> b)
        {
            b.HasKey(_ => _.ExcludedMaterialId);

            b.HasOne(_ => _.SchoolClass)
                .WithMany()
                .HasForeignKey(_ => _.SchoolClassId)
                .HasPrincipalKey(_ => _.SchoolClassId);

            b.HasOne(_ => _.Material)
                .WithMany()
                .HasForeignKey(_ => _.MaterialId)
                .HasPrincipalKey(_ => _.MaterialId);

            b.HasIndex(_ => new { _.SchoolClassId, _.MaterialId }).IsUnique();
        }
    }
}
