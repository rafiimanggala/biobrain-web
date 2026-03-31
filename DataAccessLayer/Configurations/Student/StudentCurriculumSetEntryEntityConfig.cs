using Biobrain.Domain.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentCurriculumSetEntryEntityConfig : IEntityTypeConfiguration<StudentCurriculumSetEntryEntity>
    {
        public void Configure(EntityTypeBuilder<StudentCurriculumSetEntryEntity> b)
        {
            b.HasKey(_ => new {_.StudentCurriculumSetId, _.CourseId});

            b.HasOne(_ => _.Set).WithMany(_ => _.Courses).HasForeignKey(_ => _.StudentCurriculumSetId).HasPrincipalKey(_ => _.StudentCurriculumSetId);
            b.HasOne(_ => _.Course).WithMany().HasForeignKey(_ => _.CourseId).HasPrincipalKey(_ => _.CourseId);
        }
    }
}