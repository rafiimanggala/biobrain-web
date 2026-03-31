using System.Collections.Immutable;
using Biobrain.Application.Content.ContentDataModels;

namespace Biobrain.Application.Content.Services.ContentCacheService
{
	public record ContentCacheModel
	{
		public ImmutableList<ContentData.Page> Pages { get; init; }
		public ImmutableList<ContentData.Quiz> Quizzes { get; init; }
		public ImmutableList<ContentData.ContentTree> ContentForest { get; init; }
		public ImmutableList<ContentData.GlossaryTerm> GlossaryTerms { get; init; }
	}
}
