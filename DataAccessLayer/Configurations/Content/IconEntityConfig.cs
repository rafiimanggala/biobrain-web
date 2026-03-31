using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
    public class IconEntityConfig : IEntityTypeConfiguration<IconEntity>
    {
        public void Configure(EntityTypeBuilder<IconEntity> b)
        {
            b.HasKey(_ => _.IconId);
        }
    }
}
