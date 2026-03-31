using Biobrain.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.User
{
    public class UserSessionSchoolEntityConfig : IEntityTypeConfiguration<UserSessionSchoolEntity>
    {
        public void Configure(EntityTypeBuilder<UserSessionSchoolEntity> b)
        {
            b.HasKey(_ => _.UserSessionSchoolId);

            b.HasOne(_ => _.UserSession).WithMany(_ => _.Schools).HasForeignKey(_ => _.UserSessionId).HasPrincipalKey(_ => _.UserSessionId);
            b.HasOne(_ => _.School).WithMany().HasForeignKey(_ => _.SchoolId).HasPrincipalKey(_ => _.SchoolId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}