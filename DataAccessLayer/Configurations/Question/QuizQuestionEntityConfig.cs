using Biobrain.Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Question
{
    public class QuizQuestionEntityConfig : IEntityTypeConfiguration<QuizQuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuizQuestionEntity> b)
        {
            b.HasKey(_ => new {_.QuestionId, _.QuizId});
            b.HasOne(x => x.Question)
	            .WithMany(x => x.QuizQuestions)
	            .HasForeignKey(x => x.QuestionId);
            b.HasOne(x => x.Quiz)
	            .WithMany(x => x.QuizQuestions)
	            .HasForeignKey(x => x.QuizId);
        }
    }
}
