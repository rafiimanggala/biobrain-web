using Biobrain.Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Question
{
    public class QuestionEntityConfig : IEntityTypeConfiguration<QuestionEntity>
    {
        public void Configure(EntityTypeBuilder<QuestionEntity> b)
        {
            b.HasKey(_ => _.QuestionId);
            b.HasMany(x => x.Answers)
	            .WithOne()
	            .HasForeignKey(x => x.QuestionId);
            b.HasOne(x => x.QuestionType)
	            .WithMany()
	            .HasForeignKey(x => x.QuestionTypeCode);
        }
    }
}
