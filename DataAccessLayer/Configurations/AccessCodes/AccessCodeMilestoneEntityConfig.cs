using Biobrain.Domain.Entities.AccessCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.AccessCodes
{
    public class AccessCodeMilestoneEntityConfig : IEntityTypeConfiguration<AccessCodeMilestoneEntity>
    {
        public void Configure(EntityTypeBuilder<AccessCodeMilestoneEntity> b)
        {
            b.HasKey(_ => _.AccessCodeId);
            b.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id);
        }
    }
}
