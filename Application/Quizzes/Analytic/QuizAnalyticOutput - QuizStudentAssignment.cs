using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Quizzes.Analytic
{
    public static partial class QuizAnalyticOutput
    {
        [PublicAPI]
        public record QuizStudentAssignment
        {
	        public Guid QuizStudentAssignmentId { get; init; }
	        public Guid QuizAssignmentId { get; init; }
	        public Guid AssignedToUserId { get; init; }
        }
    }
}
