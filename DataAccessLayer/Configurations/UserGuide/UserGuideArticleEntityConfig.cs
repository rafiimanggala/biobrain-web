using Biobrain.Domain.Entities.UserGuide;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.UserGuide
{
    public class UserGuideArticleEntityConfig : IEntityTypeConfiguration<UserGuideArticleEntity>
    {
        public void Configure(EntityTypeBuilder<UserGuideArticleEntity> b)
        {
            b.HasKey(_ => _.UserGuideArticleId);

            b.HasOne(_ => _.UserGuideContentTreeNode)
             .WithOne(_ => _.Article)
             .HasForeignKey<UserGuideArticleEntity>(_ => _.UserGuideContentTreeId)
             .HasPrincipalKey<UserGuideContentTreeEntity>(_ => _.NodeId);
        }
    }
}
