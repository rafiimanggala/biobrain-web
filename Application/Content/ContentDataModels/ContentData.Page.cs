using System;
using System.Collections.Immutable;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Page
        {
            public Guid PageId { get; init; }
            public Guid CourseId { get; init; }
            public Guid ContentTreeNodeId { get; init; }
            public ImmutableList<Material> Materials { get; init; }
        }
    }
}
