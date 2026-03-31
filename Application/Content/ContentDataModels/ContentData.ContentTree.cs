using System;
using System.Collections.Immutable;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record ContentTree
        {
            public Guid NodeId { get; init; }
            public Guid? ParentId { get; init; }
            public Guid CourseId { get; init; }
            public string Name { get; init; }
            public long Order { get; init; }
            public bool IsAvailableInDemo { get; init; }
            public Icon Icon { get; init; }
            public ContentTreeMeta ContentTreeMeta { get; init; }
            public ImmutableList<ContentTree> Nodes { get; init; }
        }
    }
}
