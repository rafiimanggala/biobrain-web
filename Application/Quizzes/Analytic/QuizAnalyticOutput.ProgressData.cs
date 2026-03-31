using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record ProgressData
        {
            public Guid StudentId { get; init; }
            public double Progress { get; init; }
        }
    }
}
