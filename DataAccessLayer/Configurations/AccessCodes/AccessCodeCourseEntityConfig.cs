using Biobrain.Domain.Entities.AccessCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.AccessCodes
{
    public class AccessCodeBatchCourseEntityConfig : IEntityTypeConfiguration<AccessCodeBatchCourseEntity>
    {
        public void Configure(EntityTypeBuilder<AccessCodeBatchCourseEntity> b)
        {
            b.HasKey(_ => _.AccessCodeCourseId);

            b.HasOne(_ => _.Course).WithMany().HasForeignKey(_ => _.CourseId).HasPrincipalKey(_ => _.CourseId);
            b.HasOne(_ => _.AccessCodeBatch).WithMany(_ => _.Courses).HasForeignKey(_ => _.AccessCodeBatchId).HasPrincipalKey(_ => _.AccessCodeBatchId);
        }
    }
}
