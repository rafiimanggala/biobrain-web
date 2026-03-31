using Biobrain.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Notifications
{
    public class EmailMessageEntityConfig : IEntityTypeConfiguration<EmailMessageEntity>
    {
        public void Configure(EntityTypeBuilder<EmailMessageEntity> b)
        {
            b.HasKey(_ => _.EmailMessageId);
        }
    }
}
