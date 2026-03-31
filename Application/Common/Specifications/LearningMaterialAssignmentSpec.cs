using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.MaterialAssignments;


namespace Biobrain.Application.Specifications
{
    public static class LearningMaterialAssignmentSpec
    {
        public static Spec<LearningMaterialAssignmentEntity> ByClassId(Guid id) => new(_ => _.SchoolClassId == id);
        public static Spec<LearningMaterialAssignmentEntity> ById(Guid id) => new(_ => _.LearningMaterialAssignmentId == id);

        public static Spec<LearningMaterialAssignmentEntity> AssignedByTeacher(Guid teacherId) => new(_ => _.AssignedByUserId == teacherId);

        public static Spec<LearningMaterialAssignmentEntity> ByContentTreeNodeIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.ContentTreeNodeId));

        public static Spec<LearningMaterialAssignmentEntity> FromDate(DateTime fromDateUtc) => new(_ => _.AssignedAtUtc > fromDateUtc);
    }
}
