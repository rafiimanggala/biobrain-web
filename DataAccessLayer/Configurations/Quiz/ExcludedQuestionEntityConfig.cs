using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class ExcludedQuestionEntityConfig : IEntityTypeConfiguration<ExcludedQuestionEntity>
    {
        public void Configure(EntityTypeBuilder<ExcludedQuestionEntity> b)
        {
            b.HasKey(_ => _.ExcludedQuestionId);
            b.HasOne(_ => _.Quiz).WithMany().HasForeignKey(_ => _.QuizId).HasPrincipalKey(_ => _.QuizId);
            b.HasOne(_ => _.SchoolClass).WithMany().HasForeignKey(_ => _.SchoolClassId).HasPrincipalKey(_ => _.SchoolClassId);
            b.HasOne(x => x.Question)
	            .WithMany()
	            .HasForeignKey(x => x.QuestionId);
        }
    }
}
