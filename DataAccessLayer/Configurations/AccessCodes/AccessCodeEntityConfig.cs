using Biobrain.Domain.Entities.AccessCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Course
{
    public class AccessCodeEntityConfig : IEntityTypeConfiguration<AccessCodeEntity>
    {
        public void Configure(EntityTypeBuilder<AccessCodeEntity> b)
        {
            b.HasKey(_ => _.AccessCodeId);
            b.HasIndex(_ => new {_.Code}).IsUnique();
        }
    }
}
