using Biobrain.Domain.Entities.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.School
{
    public class SchoolEntityConfig : IEntityTypeConfiguration<SchoolEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolEntity> b)
        {
            b.HasKey(_ => _.SchoolId);

            b.Property(_ => _.Name).IsRequired();
            b.HasIndex(_ => _.Name).IsUnique();

            // SAML SSO fields
            b.Property(_ => _.SsoEnabled).HasDefaultValue(false);
            b.Property(_ => _.SamlEntityId).HasMaxLength(500);
            b.Property(_ => _.SamlMetadataUrl).HasMaxLength(1000);
            b.Property(_ => _.SamlLoginUrl).HasMaxLength(1000);
            // SamlCertificate can be large (PEM), no max length constraint
        }
    }
}