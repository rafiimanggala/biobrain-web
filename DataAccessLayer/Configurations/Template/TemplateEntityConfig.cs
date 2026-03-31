using Biobrain.Domain.Entities.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Template
{
    public class TemplateEntityConfig : IEntityTypeConfiguration<TemplateEntity>
    {
        public void Configure(EntityTypeBuilder<TemplateEntity> b)
        {
            b.HasKey(_ => _.TemplateId);
        }
    }
}