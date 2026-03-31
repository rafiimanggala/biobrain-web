using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Projections;
using Biobrain.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Content.Internal
{
    internal interface IContentQuizDataBuilder
    {
        Task<ImmutableList<ContentData.Quiz>> Build(IQueryable<QuizEntity> query, CancellationToken cancellationToken);
    }

    internal sealed class ContentQuizDataBuilder : IContentQuizDataBuilder
    {
        public Task<ImmutableList<ContentData.Quiz>> Build(IQueryable<QuizEntity> query, CancellationToken cancellationToken) => query.AsNoTracking()
           .PrepareToMapToQuizContentData()
           .Select(QuizProjection.ToQuizContentData())
           .ToImmutableListAsync(cancellationToken);
    }
}
