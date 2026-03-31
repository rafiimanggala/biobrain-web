using Biobrain.Domain.Entities.Glossary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
    public class TermEntityConfig : IEntityTypeConfiguration<TermEntity>
    {
        public void Configure(EntityTypeBuilder<TermEntity> b)
        {
            b.HasKey(_ => _.TermId);

            b.HasOne(_ => _.Subject)
             .WithMany(_ => _.Glossary)
             .HasForeignKey(_ => _.SubjectCode)
             .HasPrincipalKey(_ => _.SubjectCode);
        }
    }
}
