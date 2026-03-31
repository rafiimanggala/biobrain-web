using System;
using System.Linq.Expressions;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Domain.Entities.Course;


namespace Biobrain.Application.Projections
{
    public static class CourseProjection
    {
        public static Expression<Func<CourseEntity, QuizAnalyticOutput.SubjectInfo>> ToQuizAnalyticCourse()
            => _ => new QuizAnalyticOutput.SubjectInfo
                    {
                        SubjectName = _.Subject.Name,
                        CourseId = _.CourseId
                    };
    }
}
