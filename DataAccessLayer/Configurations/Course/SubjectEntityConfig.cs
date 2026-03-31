using Biobrain.Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Course
{
    public class SubjectEntityConfig : IEntityTypeConfiguration<SubjectEntity>
    {
        public void Configure(EntityTypeBuilder<SubjectEntity> b)
        {
            b.HasKey(_ => _.SubjectCode);
            b.HasIndex(_ => _.Name).IsUnique();
        }
    }
}