using System;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Specifications
{
    public static class QuizResultSpec
    {
        public static Spec<QuizResultEntity> ByStudentAssignmentId(Guid quizStudentAssignmentId)
            => new(_ => _.QuizStudentAssignmentId == quizStudentAssignmentId);
    }
}
