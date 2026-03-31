using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record ContentTreeMeta
        {
            public Guid ContentTreeMetaId { get; init; }
            public string Name { get; init; }
            public long Depth { get; init; }
            public bool CouldAddEntry { get; init; }
            public bool CouldAddContent { get; init; }
            public bool AutoExpand { get; init; }
        }
    }
}
