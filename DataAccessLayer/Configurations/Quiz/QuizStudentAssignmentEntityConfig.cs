using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizStudentAssignmentEntityConfig : IEntityTypeConfiguration<QuizStudentAssignmentEntity>
    {
        public void Configure(EntityTypeBuilder<QuizStudentAssignmentEntity> b)
        {
            b.HasKey(_ => _.QuizStudentAssignmentId);

            b.HasOne(_ => _.QuizAssignment)
             .WithMany(_ => _.QuizStudentAssignments)
             .HasForeignKey(_ => _.QuizAssignmentId)
             .HasPrincipalKey(_ => _.QuizAssignmentId);

            b.HasOne(_ => _.AssignedTo)
             .WithMany(_ => _.AssignedQuizzes)
             .HasForeignKey(_ => _.AssignedToUserId)
             .HasPrincipalKey(_ => _.Id);
        }
    }
}
