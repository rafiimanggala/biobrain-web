using System;
using System.Linq.Expressions;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Projections
{
    public static class QuizStudentAssignmentProjection
    {
        public static Expression<Func<QuizStudentAssignmentEntity, QuizAnalyticOutput.QuizStudentAssignment>> ToQuizAnalyticQuizStudentAssignment()
            => _ => new QuizAnalyticOutput.QuizStudentAssignment
                    {
                        QuizAssignmentId = _.QuizAssignmentId,
                        QuizStudentAssignmentId = _.QuizStudentAssignmentId,
                        AssignedToUserId = _.AssignedToUserId
                    };
    }
}
