using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record Student
        {
            public Guid StudentId { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
        }
    }
}
