using Biobrain.Domain.Entities.UserGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.UserGuide
{
    public class UserGuideContentTreeEntityConfig : IEntityTypeConfiguration<UserGuideContentTreeEntity>
    {
        public void Configure(EntityTypeBuilder<UserGuideContentTreeEntity> b)
        {
            b.HasKey(_ => _.NodeId);

            b.HasOne(_ => _.Parent)
             .WithMany()
             .HasForeignKey(_ => _.ParentId)
             .HasPrincipalKey(_ => _.NodeId);
        }
    }
}
