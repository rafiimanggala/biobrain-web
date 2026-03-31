using Biobrain.Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Question
{
    public class QuizExcludedQuestionEntityConfig : IEntityTypeConfiguration<QuizExcludedQuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuizExcludedQuestionEntity> b)
        {
            b.HasKey(_ => new {_.QuestionId, _.QuizId});
            b.HasOne(x => x.Question)
	            .WithMany(x => x.QuizExcludedQuestions)
	            .HasForeignKey(x => x.QuestionId);
            b.HasOne(x => x.Quiz)
	            .WithMany(x => x.QuizExcludedQuestions)
	            .HasForeignKey(x => x.QuizId);
        }
    }
}
