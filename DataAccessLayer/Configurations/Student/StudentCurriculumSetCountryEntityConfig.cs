using Biobrain.Domain.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentCurriculumSetCountryEntityConfig : IEntityTypeConfiguration<StudentCurriculumSetCountryEntity>
    {
        public void Configure(EntityTypeBuilder<StudentCurriculumSetCountryEntity> b)
        {
            b.HasKey(_ => new {_.StudentCurriculumSetId, _.Country});

            b.HasOne(_ => _.StudentCurriculumSet).WithMany(_ => _.Countries).HasForeignKey(_ => _.StudentCurriculumSetId).HasPrincipalKey(_ => _.StudentCurriculumSetId);
        }
    }
}