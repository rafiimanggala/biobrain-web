using Biobrain.Domain.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentCurriculumSetEntityConfig : IEntityTypeConfiguration<StudentCurriculumSetEntity>
    {
        public void Configure(EntityTypeBuilder<StudentCurriculumSetEntity> b)
        {
            b.HasKey(_ => _.StudentCurriculumSetId);

            b.HasOne(_ => _.Curriculum).WithMany().HasForeignKey(_ => _.MainCurriculumCode).HasPrincipalKey(_ => _.CurriculumCode);
        }
    }
}