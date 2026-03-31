using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Content;

namespace Biobrain.Application.Services.Domain.ContentTreeService
{
	public interface IContentTreeService
	{
		Task<ImmutableDictionary<Guid, ContentTreeEntity>> GetCourseStructure(Guid courseId,
			CancellationToken cancellationToken);
	}
}