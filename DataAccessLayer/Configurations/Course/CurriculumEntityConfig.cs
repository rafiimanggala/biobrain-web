using Biobrain.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Course
{
    public class CurriculumEntityConfig : IEntityTypeConfiguration<CurriculumEntity>
    {
        public void Configure(EntityTypeBuilder<CurriculumEntity> b)
        {
            b.HasKey(_ => _.CurriculumCode);
            b.HasIndex(_ => _.Name).IsUnique();
        }
    }
}