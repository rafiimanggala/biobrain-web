using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Specifications
{
    public static class QuizStudentAssignmentSpec
    {
        public static Spec<QuizStudentAssignmentEntity> ForQuizAssignments(IEnumerable<Guid> quizAssignmentIds)
            => new(_ => quizAssignmentIds.Contains(_.QuizAssignmentId));

        public static Spec<QuizStudentAssignmentEntity> ForQuizAssignment(Guid quizAssignmentId)
            => new(_ => _.QuizAssignmentId == quizAssignmentId);

        public static Spec<QuizStudentAssignmentEntity> IsCompleted()
            => new(_ => _.Result.CompletedAt.HasValue);

        public static Spec<QuizStudentAssignmentEntity> ForUser(Guid userId)
            => new(_ => _.AssignedToUserId == userId);

        public static Spec<QuizStudentAssignmentEntity> ForUsers(IEnumerable<Guid> userIds)
            => new(_ => userIds.Contains(_.AssignedToUserId));

        public static Spec<QuizStudentAssignmentEntity> ByIds(IEnumerable<Guid> ids)
            => new(_ => ids.Contains(_.QuizStudentAssignmentId));

        public static Spec<QuizStudentAssignmentEntity> IsPartOfClassAssignment(Guid schoolClassId)
            => new(_ => _.QuizAssignment.SchoolClassId == schoolClassId);

        public static Spec<QuizStudentAssignmentEntity> ForCourse(Guid courseId)
            => new(_ => _.QuizAssignment.Quiz.ContentTreeNode.CourseId == courseId);

        public static Spec<QuizStudentAssignmentEntity> NotCompleted()
            => new(_ => !_.Result.CompletedAt.HasValue);

        public static Spec<QuizStudentAssignmentEntity> ById(Guid id)
            => new(_ => _.QuizStudentAssignmentId == id);

        public static Spec<QuizStudentAssignmentEntity> HasClassAssignment()
            => new(_ => _.QuizAssignment.SchoolClassId.HasValue);
    }
}
