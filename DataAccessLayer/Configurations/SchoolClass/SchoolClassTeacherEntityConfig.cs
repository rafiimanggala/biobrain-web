using Biobrain.Domain.Entities.SchoolClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.SchoolClass
{
    public class SchoolClassTeacherEntityConfig : IEntityTypeConfiguration<SchoolClassTeacherEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolClassTeacherEntity> b)
        {
            b.HasKey(_ => new { _.TeacherId, _.SchoolClassId });

            b.HasOne(_ => _.Teacher).WithMany(_ => _.Classes).HasForeignKey(_ => _.TeacherId).HasPrincipalKey(_ => _.TeacherId);
            b.HasOne(_ => _.SchoolClass).WithMany(_ => _.Teachers).HasForeignKey(_ => _.SchoolClassId).HasPrincipalKey(_ => _.SchoolClassId);
        }
    }
}