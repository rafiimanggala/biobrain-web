using System;
using System.Collections.Immutable;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Quiz
        {
            public Guid QuizId { get; init; }
            public Guid CourseId { get; init; }
            public ImmutableList<Question> Questions { get; init; }
            public Guid ContentTreeNodeId { get; init; }
            public string Name { get; init; }
            public int? QuestionCount { get; init; }
        }
    }
}
