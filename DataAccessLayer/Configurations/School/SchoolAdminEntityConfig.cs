using Biobrain.Domain.Entities.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.School
{
    public class SchoolAdminEntityConfig : IEntityTypeConfiguration<SchoolAdminEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolAdminEntity> b)
        {
            b.HasKey(_ => new {_.SchoolId, _.TeacherId});

            b.HasOne(_ => _.School).WithMany(_ => _.SchoolAdmins).HasForeignKey(_ => _.SchoolId).HasPrincipalKey(_ => _.SchoolId);
            b.HasOne(_ => _.Teacher).WithMany().HasForeignKey(_ => _.TeacherId).HasPrincipalKey(_ => _.TeacherId);
        }
    }
}