using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.ContentTreeService
{
	public class ContentTreeService : IContentTreeService
	{
		private readonly IDb _db;

		public ContentTreeService(IDb db) => _db = db;

        public async Task<ImmutableDictionary<Guid, ContentTreeEntity>> GetCourseStructure(Guid courseId, CancellationToken cancellationToken)
		{
			var content = await _db.ContentTree
				.Include(_ => _.ContentTreeMeta)
				.AsNoTracking()
				.Where(ContentTreeSpec.ForCourse(courseId))
				.ToListAsync(cancellationToken);

			return content.ToImmutableDictionary(_ => _.NodeId);
		}
	}
}