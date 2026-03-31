using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizAssignmentEntityConfig : IEntityTypeConfiguration<QuizAssignmentEntity>
    {
        public void Configure(EntityTypeBuilder<QuizAssignmentEntity> b)
        {
            b.HasKey(_ => _.QuizAssignmentId);

            b.HasOne(_ => _.Quiz)
             .WithMany(_ => _.Assignments)
             .HasForeignKey(_ => _.QuizId)
             .HasPrincipalKey(_ => _.QuizId);

            b.HasOne(_ => _.AssignedBy)
             .WithMany(_ => _.AssignedByTeacherQuizzes)
             .HasForeignKey(_ => _.AssignedByTeacherId)
             .HasPrincipalKey(_ => _.TeacherId);

            b.HasOne(_ => _.SchoolClass)
             .WithMany(_ => _.AssignedQuizzes)
             .HasForeignKey(_ => _.SchoolClassId)
             .HasPrincipalKey(_ => _.SchoolClassId);
        }
    }
}
