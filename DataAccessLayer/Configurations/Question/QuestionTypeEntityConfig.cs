using Biobrain.Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Question
{
    public class QuestionTypeEntityConfig : IEntityTypeConfiguration<QuestionTypeEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionTypeEntity> b)
        {
            b.HasKey(_ => _.QuestionTypeCode);
        }
    }
}
