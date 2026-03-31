using Biobrain.Domain.Entities.SchoolClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.SchoolClass
{
    public class SchoolClassStudentsEntityConfig : IEntityTypeConfiguration<SchoolClassStudentEntity>
    {
        public void Configure(EntityTypeBuilder<SchoolClassStudentEntity> b)
        {
            b.HasKey(_ => new {_.SchoolClassId, _.StudentId});
        }
    }
}