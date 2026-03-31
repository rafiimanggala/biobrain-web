using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizResultEntityConfig : IEntityTypeConfiguration<QuizResultEntity>
    {
        public void Configure(EntityTypeBuilder<QuizResultEntity> b)
        {
            b.HasKey(_ => _.QuizStudentAssignmentId);

            b.HasOne(_ => _.QuizStudentAssignment)
             .WithOne(_ => _.Result)
             .HasForeignKey<QuizResultEntity>(_ => _.QuizStudentAssignmentId);
        }
    }
}
