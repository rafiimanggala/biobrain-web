using Biobrain.Domain.Entities.AccessCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Course
{
    public class AccessCodeBatchEntityConfig : IEntityTypeConfiguration<AccessCodeBatchEntity>
    {
        public void Configure(EntityTypeBuilder<AccessCodeBatchEntity> b)
        {
            b.HasKey(_ => _.AccessCodeBatchId);

            b.HasMany(_ => _.AccessCodes).WithOne(_ => _.Batch).HasForeignKey(_ => _.BatchId).HasPrincipalKey(_ => _.AccessCodeBatchId);
            b.HasMany(_ => _.UsedAccessCodes).WithOne(_ => _.Batch).HasForeignKey(_ => _.BatchId).HasPrincipalKey(_ => _.AccessCodeBatchId);
        }
    }
}
