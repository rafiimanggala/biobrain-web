using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Answer
        {
            public Guid AnswerId { get; init; }
            public int AnswerOrder { get; init; }
            public string Text { get; init; }
            public bool IsCorrect { get; init; }
            public bool CaseSensitive { get; init; }
            public int Score { get; init; }
            public int Response { get; init; }
        }
    }
}
