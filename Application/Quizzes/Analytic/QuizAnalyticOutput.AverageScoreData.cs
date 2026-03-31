using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record AverageScoreData
        {
            public Guid StudentId { get; init; }
            public double AverageScore { get; init; }
            public string NotApplicable { get; init; }
        }
    }
}
