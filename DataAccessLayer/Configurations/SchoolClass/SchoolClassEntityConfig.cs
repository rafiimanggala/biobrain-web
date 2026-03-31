using Biobrain.Domain.Entities.SchoolClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.SchoolClass
{
    public class SchoolClassEntityConfig : IEntityTypeConfiguration<SchoolClassEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolClassEntity> b)
        {
            b.HasKey(_ => _.SchoolClassId);

            b.HasOne(_ => _.School).WithMany(_ => _.Classes).HasForeignKey(_ => _.SchoolId).HasPrincipalKey(_ => _.SchoolId);

            b.HasMany(_ => _.Students).WithOne(_ => _.SchoolClass).HasForeignKey(_ => _.SchoolClassId).HasPrincipalKey(_ => _.SchoolClassId);

            b.HasIndex(_ => new {_.SchoolId, _.Year, _.Name}).IsUnique();

            // must be unique through all schools to allow a student to sign up to specific class of a specific school.
            b.HasIndex(_ => _.AutoJoinClassCode).IsUnique();
        }
    }
}
