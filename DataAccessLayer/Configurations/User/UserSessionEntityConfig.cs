using Biobrain.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.User
{
    public class UserSessionEntityConfig : IEntityTypeConfiguration<UserSessionEntity>
    {
        public void Configure(EntityTypeBuilder<UserSessionEntity> b)
        {
            b.HasKey(_ => _.UserSessionId);

            b.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id);
        }
    }
}