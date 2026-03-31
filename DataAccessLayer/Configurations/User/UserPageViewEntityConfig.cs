using Biobrain.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.User
{
    public class UserPageViewEntityConfig : IEntityTypeConfiguration<UserPageViewEntity>
    {
        public void Configure(EntityTypeBuilder<UserPageViewEntity> b)
        {
            b.HasKey(_ => _.UserPageViewId);

            b.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id);
            b.HasOne(_ => _.Course).WithMany().HasForeignKey(_ => _.CourseId).HasPrincipalKey(_ => _.CourseId);
            b.HasOne(_ => _.School).WithMany().HasForeignKey(_ => _.SchoolId).HasPrincipalKey(_ => _.SchoolId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(_ => _.PagePath);
        }
    }
}