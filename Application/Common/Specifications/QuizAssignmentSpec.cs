using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Specifications
{
    public static class QuizAssignmentSpec
    {
        public static Spec<QuizAssignmentEntity> ForClass(Guid classId) => new(_ => _.SchoolClassId == classId);
        public static Spec<QuizAssignmentEntity> ForCourse(Guid courseId) => new(_ => _.Quiz.ContentTreeNode.CourseId == courseId);
        public static Spec<QuizAssignmentEntity> ById(Guid id) => new(_ => _.QuizAssignmentId == id);
        public static Spec<QuizAssignmentEntity> ByIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.QuizAssignmentId));
        public static Spec<QuizAssignmentEntity> ByClassIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.SchoolClassId.Value));
        public static Spec<QuizAssignmentEntity> ByQuizIds(IEnumerable<Guid> ids) => new(_ => ids.Contains(_.QuizId));
        public static Spec<QuizAssignmentEntity> HasClassAssignment() => new(_ => _.SchoolClassId != null);
        public static Spec<QuizAssignmentEntity> AssignedByTeacher(Guid teacherId) => new(_ => _.AssignedByTeacherId == teacherId);
        public static Spec<QuizAssignmentEntity> FromDate(DateTime fromDateUtc) => new(_ => _.AssignedAtUtc > fromDateUtc);
    }
}
