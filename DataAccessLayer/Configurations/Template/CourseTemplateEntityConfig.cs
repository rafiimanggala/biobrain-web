using Biobrain.Domain.Entities.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Template
{
    public class CourseTemplateEntityConfig : IEntityTypeConfiguration<CourseTemplateEntity>
    {
        public void Configure(EntityTypeBuilder<CourseTemplateEntity> b)
        {
            b.HasKey(_ => _.CourseTemplateId);

            b.HasOne(_ => _.Course).WithMany(_ => _.Templates).HasForeignKey(_ => _.CourseId);
            b.HasOne(_ => _.Template).WithMany().HasForeignKey(_ => _.TemplateId);
        }
    }
}