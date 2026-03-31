using Biobrain.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Course
{
    public class CourseEntityConfig : IEntityTypeConfiguration<CourseEntity>
    {
        public void Configure(EntityTypeBuilder<CourseEntity> b)
        {
            b.HasKey(_ => _.CourseId);
            b.HasIndex(_ => new {_.SubjectCode, _.CurriculumCode, _.Year});

            b.HasOne(_ => _.Subject).WithMany().HasForeignKey(_ => _.SubjectCode).HasPrincipalKey(_ => _.SubjectCode);
            b.HasOne(_ => _.Curriculum).WithMany().HasForeignKey(_ => _.CurriculumCode).HasPrincipalKey(_ => _.CurriculumCode);
        }
    }
}
