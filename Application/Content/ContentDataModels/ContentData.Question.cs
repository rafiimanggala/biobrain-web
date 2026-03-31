using System;
using System.Collections.Immutable;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Question
        {
            public Guid QuestionId { get; init; }
            public long QuestionTypeCode { get; init; }
            public string QuestionTypeName { get; init; }
            public string Header { get; init; }
            public string Text { get; init; }
            public string Hint { get; init; }
            public string FeedBack { get; init; }
            public ImmutableList<Answer> Answers { get; init; }
            public int Order { get; init; }
        }
    }
}
