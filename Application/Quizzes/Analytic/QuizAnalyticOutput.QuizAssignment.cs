using System;
using System.Collections.Generic;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record QuizAssignment
        {
            public Guid QuizId { get; init; }
            public Guid QuizAssignmentId { get; init; }
            public Guid ContentTreeNodeId { get; init; }
            public Guid? AssignedByTeacherId { get; init; }
            public List<string> Path { get; set; }
            public string QuizName { get; set; }
            public DateTime? DateUtc { get; set; }
        }
    }
}
