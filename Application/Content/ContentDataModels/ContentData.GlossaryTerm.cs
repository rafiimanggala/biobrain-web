using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record GlossaryTerm
        {
            public Guid TermId { get; init; }
            public int SubjectCode { get; init; }
            public string Ref { get; init; }
            public string Term { get; init; }
            public string Definition { get; init; }
            public string Header { get; init; }
        }
    }
}
