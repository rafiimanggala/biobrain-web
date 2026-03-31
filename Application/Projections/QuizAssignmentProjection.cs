using System;
using System.Linq.Expressions;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Domain.Entities.Quiz;


namespace Biobrain.Application.Projections
{
    public static class QuizAssignmentProjection
    {
        public static Expression<Func<QuizAssignmentEntity, QuizAnalyticOutput.QuizAssignment>> ToQuizAnalyticQuizAssignment()
            => _ => new QuizAnalyticOutput.QuizAssignment
                    {
                        ContentTreeNodeId = _.Quiz.ContentTreeId,
                        QuizId = _.QuizId,
                        QuizAssignmentId = _.QuizAssignmentId,
                        AssignedByTeacherId =  _.AssignedByTeacherId,
                        DateUtc = _.AssignedAtUtc
                    };
    }
}
