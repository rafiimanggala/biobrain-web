using Biobrain.Domain.Entities.SiteIdentity;
using Biobrain.Domain.Entities.Teacher;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Teacher
{
    public class TeacherEntityConfig : IEntityTypeConfiguration<TeacherEntity>
    {
        public void Configure(EntityTypeBuilder<TeacherEntity> b)
        {
            b.HasKey(_ => _.TeacherId);

            b.HasOne(_ => _.User).WithOne(_ => _.Teacher).HasForeignKey<TeacherEntity>(_ => _.TeacherId).HasPrincipalKey<UserEntity>(_ => _.Id);
            b.HasMany(_ => _.Schools).WithOne(_ => _.Teacher).HasForeignKey(_ => _.TeacherId).HasPrincipalKey(_ => _.TeacherId);
        }
    }
}