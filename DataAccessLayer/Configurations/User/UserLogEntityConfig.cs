using Biobrain.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.User
{
    public class UserLogEntityConfig : IEntityTypeConfiguration<UserLogEntity>
    {
        public void Configure(EntityTypeBuilder<UserLogEntity> b)
        {
            b.HasKey(_ => _.UserLogId);

            b.HasOne(_ => _.User).WithMany().HasForeignKey(_ => _.UserId).HasPrincipalKey(_ => _.Id);
        }
    }
}