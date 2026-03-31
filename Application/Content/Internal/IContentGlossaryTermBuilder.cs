using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Projections;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Glossary;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Content.Internal
{
    internal interface IContentGlossaryTermBuilder
    {
        Task<ImmutableList<ContentData.GlossaryTerm>> Build(IQueryable<TermEntity> query, CancellationToken cancellationToken);
    }


    internal sealed class ContentGlossaryTermBuilder : IContentGlossaryTermBuilder
    {
        public Task<ImmutableList<ContentData.GlossaryTerm>> Build(IQueryable<TermEntity> query, CancellationToken cancellationToken) => query.AsNoTracking()
           .Where(DeletedSpec<TermEntity>.NotDeleted())
           .Select(TermProjection.ToGlossaryTermContentData())
           .ToImmutableListAsync(cancellationToken);
    }
}
