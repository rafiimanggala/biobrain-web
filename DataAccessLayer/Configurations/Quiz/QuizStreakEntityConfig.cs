using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.Quiz
{
    public class QuizStreakEntityConfig : IEntityTypeConfiguration<QuizStreakEntity>
    {
        public void Configure(EntityTypeBuilder<QuizStreakEntity> b)
        {
            b.HasKey(_ => _.QuizStreakId);
            b.HasOne(x => x.Student)
	            .WithMany()
	            .HasForeignKey(x => x.StudentId);
            b.HasOne(x => x.Course)
	            .WithMany()
	            .HasForeignKey(x => x.CourseId);
            b.Property(x => x.UpdatedAtLocal)
             .HasColumnType("timestamp without time zone");
        }
    }
}
