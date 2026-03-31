using System;
using System.Linq.Expressions;
using Biobrain.Application.Quizzes.Analytic;
using Biobrain.Domain.Entities.SchoolClass;


namespace Biobrain.Application.Projections
{
    public static class SchoolClassProjection
    {
        public static Expression<Func<SchoolClassEntity, QuizAnalyticOutput.SchoolClassInfo>> ToQuizAnalyticClass()
            => _ => new QuizAnalyticOutput.SchoolClassInfo
                    {
                        SchoolClassId = _.SchoolClassId,
                        SchoolClassName = _.Name,
                        SchoolClassYear = _.Year
                    };
    }
}