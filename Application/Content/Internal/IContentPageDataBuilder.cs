using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Projections;
using Biobrain.Domain.Entities.Material;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Content.Internal
{
    internal interface IContentPageDataBuilder
    {
        Task<ImmutableList<ContentData.Page>> Build(IQueryable<PageEntity> query, CancellationToken cancellationToken);
    }

    internal sealed class ContentPageDataBuilder : IContentPageDataBuilder
    {
        public Task<ImmutableList<ContentData.Page>> Build(IQueryable<PageEntity> query, CancellationToken cancellationToken) => query.AsNoTracking()
           .PrepareToMapToPageContentData()
           .Select(PageProjection.ToPageContentData())
           .ToImmutableListAsync(cancellationToken);
    }
}
