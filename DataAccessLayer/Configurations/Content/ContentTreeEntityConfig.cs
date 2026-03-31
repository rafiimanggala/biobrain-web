using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
    public class ContentTreeEntityConfig : IEntityTypeConfiguration<ContentTreeEntity>
    {
        public void Configure(EntityTypeBuilder<ContentTreeEntity> b)
        {
            b.HasKey(_ => _.NodeId);

            b.HasOne(x => x.ContentTreeMeta)
             .WithMany()
             .HasForeignKey(x => x.ContentTreeMetaId);

            b.HasOne(x => x.Icon)
             .WithMany()
             .HasForeignKey(x => x.IconId);

            b.HasOne(_ => _.ParentContentTree)
             .WithMany()
             .HasForeignKey(_ => _.ParentId)
             .HasPrincipalKey(_ => _.NodeId);
        }
    }
}
