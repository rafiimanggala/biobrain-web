using Biobrain.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.User
{
    public class UserSessionCourseEntityConfig : IEntityTypeConfiguration<UserSessionCourseEntity>
    {
        public void Configure(EntityTypeBuilder<UserSessionCourseEntity> b)
        {
            b.HasKey(_ => _.UserSessionCourseEntityId);

            b.HasOne(_ => _.UserSession).WithMany(_ => _.Courses).HasForeignKey(_ => _.UserSessionId).HasPrincipalKey(_ => _.UserSessionId);
            b.HasOne(_ => _.Course).WithMany().HasForeignKey(_ => _.CourseId);
        }
    }
}