using Biobrain.Domain.Entities.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.School
{
    public class SchoolStudentEntityConfig : IEntityTypeConfiguration<SchoolStudentEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolStudentEntity> b)
        {
            b.HasKey(_ => new { _.SchoolId, _.StudentId });
        }
    }
}