using System;
using System.Collections.Generic;
using System.Linq;
using Biobrain.Domain.Entities.Content;


namespace Biobrain.Application.Services
{
    internal interface IContentTreePathResolver
    {
        List<string> ResolvePath(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure);
        List<PathItem> ResolveFullPath(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure);

        Guid EvalRoot(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure);
    }

    internal record PathItem
    {
	    public string Value;
	    public int Index;
        public long OrderInTree;
        public Guid NodeId;
    }

    internal sealed class ContentTreePathResolver : IContentTreePathResolver
    {
	    public const string Unit = "unit";
	    public const string Module = "module";
	    public const string Level = "level";
	    public const string Area = "area";
	    public const string Topic = "topic";
	    public const string Theme = "theme";
	    public const string IAAnalyst = "ia analysis";

        public List<string> ResolvePath(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure)
        {
            if (courseStructure is null)
                throw new ArgumentNullException(nameof(courseStructure));

            var result = new List<string>();
            var node = courseStructure.GetValueOrDefault(nodeId);

            while (node != null)
            {
	            var nodeName = node.Name;
                //ToDo: redo. Need to display only index for Modules, Units, Areas and levels
	            if (node.ContentTreeMeta.Name.ToLower().Contains(Module) ||node.ContentTreeMeta.Name.ToLower().Contains(Unit) || node.ContentTreeMeta.Name.ToLower().Contains(Level) || node.ContentTreeMeta.Name.ToLower().Contains(Area)
                    || (node.ContentTreeMeta.Name.ToLower().Contains(Topic) && node.ContentTreeMeta.Depth == 0))
	            {
                    var index = courseStructure.Values
                                               .Where(x => x.ContentTreeMetaId == node.ContentTreeMetaId && x.ParentId == node.ParentId)
                                               .OrderBy(x => x.Order)
                                               .ToList()
                                               .IndexOf(node);
                    nodeName = (index + 1 + node.ContentTreeMeta.StartIndex).ToString();
	            }

                //ToDo: redo. Need to display only letter for Themes
                if (node.ContentTreeMeta.Name.ToLower().Contains(Theme)  && node.ContentTreeMeta.Depth == 0)
                {
                    nodeName = node.Name.ToLower().Contains(IAAnalyst) ? node.Name[..2] : node.Name[..1];
                }

                result.Insert(0, nodeName);
                node = node.ParentId.HasValue ? courseStructure.GetValueOrDefault(node.ParentId.Value) : null;
            }

            return result;
        }

        public List<PathItem> ResolveFullPath(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure)
        {
	        if (courseStructure is null)
		        throw new ArgumentNullException(nameof(courseStructure));

	        var result = new List<PathItem>();
	        var node = courseStructure.GetValueOrDefault(nodeId);

	        while (node != null)
	        {
		        var nodeName = node.Name;
		        var index = courseStructure.Values
			        .Where(x => x.ContentTreeMetaId == node.ContentTreeMetaId && x.ParentId == node.ParentId)
			        .OrderBy(x => x.Order)
			        .ToList()
			        .IndexOf(node);
		        index = index + 1 + node.ContentTreeMeta.StartIndex;

                result.Insert(0, new PathItem{Index = index, Value = nodeName, OrderInTree = node.Order, NodeId = node.NodeId});
		        node = node.ParentId.HasValue ? courseStructure.GetValueOrDefault(node.ParentId.Value) : null;
	        }

	        return result;
        }

        public Guid EvalRoot(Guid nodeId, IReadOnlyDictionary<Guid, ContentTreeEntity> courseStructure)
        {
            var node = courseStructure.GetValueOrDefault(nodeId);

            while (node is { ParentId: { } })
                node = courseStructure.GetValueOrDefault(node.ParentId.Value);

            return node?.NodeId ?? Guid.Empty;
        }
    }
}
