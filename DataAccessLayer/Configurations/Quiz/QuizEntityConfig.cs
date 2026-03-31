using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizEntityConfig : IEntityTypeConfiguration<QuizEntity>
    {
        public void Configure(EntityTypeBuilder<QuizEntity> b)
        {
            b.HasKey(_ => _.QuizId);
            b.HasOne(_ => _.AutoMapQuiz).WithMany().HasForeignKey(_ => _.AutoMapQuizId).HasPrincipalKey(_ => _.QuizId);
            b.HasMany(x => x.QuizQuestions)
	            .WithOne()
	            .HasForeignKey(x => x.QuizId);
            b.HasOne(x => x.ContentTreeNode)
	            .WithMany()
	            .HasForeignKey(x => x.ContentTreeId);
        }
    }
}
