using System;
using System.Threading;
using System.Threading.Tasks;

namespace Biobrain.Application.Content.Services.ContentCacheService
{
	public interface IContentCacheService
	{
		Task<string> CreateContentFile(Guid courseId, long dateTime, CancellationToken cancellationToken);
		Task<ContentCacheModel> GetLatestContentFromFile(Guid courseId, CancellationToken cancellationToken);
		Task<int> DeleteOldFiles(CancellationToken cancellationToken = new());
	}
}