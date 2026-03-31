using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record SchoolClassInfo
        {
            public Guid SchoolClassId { get; init; }
            public string SchoolClassName { get; init; }
            public int SchoolClassYear { get; init; }
        }
    }
}
