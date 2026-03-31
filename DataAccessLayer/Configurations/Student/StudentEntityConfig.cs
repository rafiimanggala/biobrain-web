using Biobrain.Domain.Entities.SiteIdentity;
using Biobrain.Domain.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentEntityConfig : IEntityTypeConfiguration<StudentEntity>
    {
        public void Configure(EntityTypeBuilder<StudentEntity> b)
        {
            b.HasKey(_ => _.StudentId);

            b.HasOne(_ => _.User).WithOne(_ => _.Student).HasForeignKey<StudentEntity>(_ => _.StudentId).HasPrincipalKey<UserEntity>(_ => _.Id);
            b.HasOne(_ => _.Curriculum).WithMany().HasForeignKey(_ => _.CurriculumCode).HasPrincipalKey(_ => _.CurriculumCode);

            b.HasMany(_ => _.SchoolClasses).WithOne(_ => _.Student).HasForeignKey(_ => _.StudentId).HasPrincipalKey(_ => _.StudentId);
            b.HasMany(_ => _.Schools).WithOne(_ => _.Student).HasForeignKey(_ => _.StudentId).HasPrincipalKey(_ => _.StudentId);
        }
    }
}