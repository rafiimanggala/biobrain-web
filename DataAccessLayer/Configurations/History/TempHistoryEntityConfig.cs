using Biobrain.Domain.Entities.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.History
{
    public class TempHistoryEntityConfig : IEntityTypeConfiguration<TempHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<TempHistoryEntity> b)
        {
            b.HasKey(_ => _.TempHistoryId);
        }
    }
}
