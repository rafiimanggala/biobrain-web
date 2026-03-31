using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizTemplateEntityConfig : IEntityTypeConfiguration<QuizTemplateEntity>
    {
        public void Configure(EntityTypeBuilder<QuizTemplateEntity> b)
        {
            b.HasKey(_ => _.TemplateId);
        }
    }
}
