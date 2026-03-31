using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Material
{
    public class MaterialEntityConfig : IEntityTypeConfiguration<MaterialEntity>
    {
        public void Configure(EntityTypeBuilder<MaterialEntity> b)
        {
            b.HasKey(_ => _.MaterialId);
        }
    }
}
