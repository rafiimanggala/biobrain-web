using JetBrains.Annotations;


namespace Biobrain.Application.Content.ContentDataModels
{
    public static partial class ContentData
    {
        [PublicAPI]
        public record Icon
        {
            public string Reference { get; init; }
            public string Name { get; init; }
            public string FileName { get; init; }
        }
    }
}
