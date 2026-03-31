using DataAccessLayer.WebAppEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Question
{
    public class AnswerEntityConfig : IEntityTypeConfiguration<AnswerEntity>
    {
        public void Configure(EntityTypeBuilder<AnswerEntity> b)
        {
            b.HasKey(_ => _.AnswerId);
        }
    }
}
