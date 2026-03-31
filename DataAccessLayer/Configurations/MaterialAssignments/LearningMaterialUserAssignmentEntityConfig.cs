using Biobrain.Domain.Entities.MaterialAssignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.MaterialAssignments
{
    public class LearningMaterialUserAssignmentEntityConfig : IEntityTypeConfiguration<LearningMaterialUserAssignmentEntity>
    {
        public void Configure(EntityTypeBuilder<LearningMaterialUserAssignmentEntity> b)
        {
            b.HasKey(_ => _.LearningMaterialUserAssignmentId);

            b.HasOne(_ => _.LearningMaterialAssignment)
             .WithMany(_ => _.UserAssignments)
             .HasForeignKey(_ => _.LearningMaterialAssignmentId)
             .HasPrincipalKey(_ => _.LearningMaterialAssignmentId);

            b.HasOne(_ => _.AssignedTo)
             .WithMany(_ => _.AssignedMaterials)
             .HasForeignKey(_ => _.AssignedToUserId)
             .HasPrincipalKey(_ => _.Id);
        }
    }
}