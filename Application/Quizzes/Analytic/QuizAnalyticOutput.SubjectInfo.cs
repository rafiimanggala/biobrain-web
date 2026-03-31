using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record SubjectInfo
        {
            public string SubjectName { get; init; }
            public Guid CourseId { get; init; }
        }
    }
}
