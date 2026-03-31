using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Content
{
	class ContentVersionConfig:IEntityTypeConfiguration<ContentVersionEntity>
	{
		public void Configure(EntityTypeBuilder<ContentVersionEntity> b)
		{
			b.HasKey(_ => _.ContentVersionId);
			b.HasIndex(_ => new { _.Version });
			b.HasOne(_ => _.Course).WithOne(_ => _.Version);
		}
		
	}
}
