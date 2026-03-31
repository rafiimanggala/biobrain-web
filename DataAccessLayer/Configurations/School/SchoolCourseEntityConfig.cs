using Biobrain.Domain.Entities.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.School
{
    public class SchoolCourseEntityConfig : IEntityTypeConfiguration<SchoolCourseEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolCourseEntity> b)
        {
            b.HasKey(_ => new { _.SchoolCourseId });

            b.HasOne(_ => _.Course).WithMany().HasForeignKey(_ => _.CourseId).HasPrincipalKey(_ => _.CourseId);
            b.HasOne(_ => _.School).WithMany(_ => _.Courses).HasForeignKey(_ => _.SchoolId).HasPrincipalKey(_ => _.SchoolId);
        }
    }
}