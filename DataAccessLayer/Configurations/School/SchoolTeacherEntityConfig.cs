using Biobrain.Domain.Entities.School;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.School
{
    public class SchoolTeacherEntityConfig : IEntityTypeConfiguration<SchoolTeacherEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolTeacherEntity> b)
        {
            b.HasKey(_ => new { _.SchoolId, _.TeacherId });
        }
    }
}