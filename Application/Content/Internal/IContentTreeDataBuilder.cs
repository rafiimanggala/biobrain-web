using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Content.Internal
{
    internal interface IContentTreeDataBuilder
    {
        Task<ImmutableList<ContentData.ContentTree>> Build(IQueryable<ContentTreeEntity> query, CancellationToken cancellationToken);
    }

    internal sealed class ContentTreeDataBuilder : IContentTreeDataBuilder
    {
        public async Task<ImmutableList<ContentData.ContentTree>> Build(IQueryable<ContentTreeEntity> query, CancellationToken cancellationToken)
        {
            var contentTreeFlatList = await query.AsNoTracking()
                                                 .Include(_ => _.ContentTreeMeta)
                                                 .Include(_ => _.Icon)
                                                 .Where(DeletedSpec<ContentTreeEntity>.NotDeleted())
                                                 .ToListAsync(cancellationToken);

            var mapByParent = contentTreeFlatList.Where(_ => _.ParentId.HasValue)
                                                 .ToLookup(_ => _.ParentId.Value);

            var roots = contentTreeFlatList.Where(_ => !_.ParentId.HasValue);

            ContentData.ContentTree BuildNode(ContentTreeEntity entity)
            {
                var builder = ImmutableList.CreateBuilder<ContentData.ContentTree>();

                var children = mapByParent[entity.NodeId];
                
                // If parent available in demo and all children not available in demo
                // then need to include all of it
                bool forceAvailableInDemo = !children.Any(_ => _.IsAvailableInDemo) && entity.IsAvailableInDemo;

                foreach (var child in children)
                {
                    if(forceAvailableInDemo) child.IsAvailableInDemo = true;
                    builder.Add(BuildNode(child));
                }

                if (builder.Any(_ => _.IsAvailableInDemo))
                    entity.IsAvailableInDemo = true;

                return entity.ToContentData(builder.ToImmutable());
            }

            return roots.Select(BuildNode).ToImmutableList();
        }
    }
}
