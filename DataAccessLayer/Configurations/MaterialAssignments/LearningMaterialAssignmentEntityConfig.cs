using Biobrain.Domain.Entities.MaterialAssignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DataAccessLayer.Configurations.MaterialAssignments
{
    public class LearningMaterialAssignmentEntityConfig : IEntityTypeConfiguration<LearningMaterialAssignmentEntity>
    {
        public void Configure(EntityTypeBuilder<LearningMaterialAssignmentEntity> b)
        {
            b.HasKey(_ => _.LearningMaterialAssignmentId);

            b.HasOne(_ => _.ContentTreeNode)
             .WithMany(_ => _.Assignments)
             .HasForeignKey(_ => _.ContentTreeNodeId)
             .HasPrincipalKey(_ => _.NodeId);

            b.HasOne(_ => _.AssignedBy)
             .WithMany(_ => _.AssignedByUserMaterials)
             .HasForeignKey(_ => _.AssignedByUserId)
             .HasPrincipalKey(_ => _.Id);

            b.HasOne(_ => _.SchoolClass)
             .WithMany(_ => _.AssignedMaterials)
             .HasForeignKey(_ => _.SchoolClassId)
             .HasPrincipalKey(_ => _.SchoolClassId);
        }
    }
}