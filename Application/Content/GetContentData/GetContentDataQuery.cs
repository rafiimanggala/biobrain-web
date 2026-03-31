using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Content.ContentDataModels;
using Biobrain.Application.Content.Services.ContentCacheService;
using Biobrain.Application.Interfaces.DataAccess;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;


namespace Biobrain.Application.Content.GetContentData
{
    [Obsolete("Will be removed in future releases. Returns empty data for the sake of backward compatibility.")]
    [PublicAPI]
    public sealed class GetContentDataQuery : IQuery<GetContentDataQuery.Result>
    {
	    [PublicAPI]
        public record Result
        {
            public ImmutableList<ContentData.Page> Pages { get; init; }
            public ImmutableList<ContentData.Quiz> Quizzes { get; init; }
            public ImmutableList<ContentData.ContentTree> ContentForest { get; init; }
            public ImmutableList<ContentData.GlossaryTerm> GlossaryTerms { get; init; }
        }

        internal sealed class Handler(IDb db,
                                      IConfiguration configuration,
                                      IContentCacheService contentCacheService)
            : QueryHandlerBase<GetContentDataQuery, Result>(db)
        {
            //private readonly IContentTreeDataBuilder _contentTreeDataBuilder;
            //private readonly IContentPageDataBuilder _contentPageDataBuilder;
            //private readonly IContentQuizDataBuilder _contentQuizDataBuilder;
            //private readonly IContentGlossaryTermBuilder _contentGlossaryTermBuilder;
            private readonly IConfiguration _configuration = configuration;
            private readonly IContentCacheService _contentCacheService = contentCacheService;

            public override Task<Result> Handle(GetContentDataQuery request, CancellationToken cancellationToken) =>
                //var content = await _contentCacheService.GetLatestContentFromFile(cancellationToken);
                Task.FromResult(new Result
                                {
                                    //Quizzes = content.Quizzes,
                                    //ContentForest = content.ContentForest,
                                    //GlossaryTerms = content.GlossaryTerms,
                                    //Pages = content.Pages
                                });
        }
    }
}
