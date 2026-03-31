using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Content.Services.ContentCacheService;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;


namespace Biobrain.Application.Content.GetCourseContentData
{
    [PublicAPI]
    public sealed class GetCourseContentDataQuery : IQuery<GetCourseContentDataQuery.Result>
    {
        public Guid CourseId { get; init; }

        [PublicAPI]
        public record Result
        {
            public ImmutableList<ContentData.Page> Pages { get; init; }
            public ImmutableList<ContentData.Quiz> Quizzes { get; init; }
            public ImmutableList<ContentData.ContentTree> ContentForest { get; init; }
            public ImmutableList<ContentData.GlossaryTerm> GlossaryTerms { get; init; }
        }


        internal sealed class Handler : QueryHandlerBase<GetCourseContentDataQuery, Result>
        {
            private readonly IContentCacheService _contentCacheService;

            public Handler(IDb db, IContentCacheService contentCacheService) : base(db) => _contentCacheService = contentCacheService;

            public override async Task<Result> Handle(GetCourseContentDataQuery request, CancellationToken cancellationToken)
            {
	            var content = await _contentCacheService.GetLatestContentFromFile(request.CourseId, cancellationToken);

	            return new Result
	            {
		            Quizzes = content.Quizzes,
		            ContentForest = content.ContentForest,
		            GlossaryTerms = content.GlossaryTerms,
		            Pages = content.Pages
	            };
            }
        }
    }
}
