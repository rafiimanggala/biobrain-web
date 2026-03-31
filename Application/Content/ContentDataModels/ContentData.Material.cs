using System;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Material
        {
            public Guid MaterialId { get; init; }
            public string Text { get; init; }
            public string Header { get; init; }
            public string VideoLink { get; set; }
            public int Order { get; init; }
        }
    }
}
