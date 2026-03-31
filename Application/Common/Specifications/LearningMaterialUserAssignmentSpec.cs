using System;
using Biobrain.Domain.Entities.MaterialAssignments;


namespace Biobrain.Application.Specifications
{
    public static class LearningMaterialUserAssignmentSpec
    {
        public static Spec<LearningMaterialUserAssignmentEntity> ById(Guid id) => new(_ => _.LearningMaterialUserAssignmentId == id);

        public static Spec<LearningMaterialUserAssignmentEntity> ByUserId(Guid userId) => new(_ => _.AssignedToUserId == userId);

        public static Spec<LearningMaterialUserAssignmentEntity> ByCourseId(Guid courseId) 
            => new(_ => _.LearningMaterialAssignment.ContentTreeNode.CourseId == courseId);

        public static Spec<LearningMaterialUserAssignmentEntity> NotCompleted()
            => new(_ => !_.CompletedAtUtc.HasValue);

        public static Spec<LearningMaterialUserAssignmentEntity> IsCompleted()
            => new(_ => _.CompletedAtUtc.HasValue);

        public static Spec<LearningMaterialUserAssignmentEntity> BySchoolClassId(Guid schoolClassId)
            => new(_ => _.LearningMaterialAssignment.SchoolClassId == schoolClassId);
    }
}
