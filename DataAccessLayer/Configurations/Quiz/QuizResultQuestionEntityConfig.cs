using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizResultQuestionEntityConfig : IEntityTypeConfiguration<QuizResultQuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuizResultQuestionEntity> b)
        {
            b.HasKey(_ => new {_.QuizResultId, _.QuestionId});

            b.HasOne(_ => _.QuizResult)
             .WithMany(_ => _.Questions)
             .HasForeignKey(_ => _.QuizResultId)
             .HasPrincipalKey(_ => _.QuizStudentAssignmentId);

            b.HasOne(_ => _.Question)
             .WithMany(_ => _.QuizResultQuestions)
             .HasForeignKey(_ => _.QuestionId)
             .HasPrincipalKey(_ => _.QuestionId);
        }
    }
}
