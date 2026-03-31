using DataAccessLayer.WebAppEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
    public class ContentTreeMetaEntityConfig : IEntityTypeConfiguration<ContentTreeMetaEntity>
    {
        public void Configure(EntityTypeBuilder<ContentTreeMetaEntity> b)
        {
            b.HasKey(_ => _.ContentTreeMetaId);
        }
    }
}
