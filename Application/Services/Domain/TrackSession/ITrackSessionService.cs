using System;
using System.Threading;
using System.Threading.Tasks;

namespace Biobrain.Application.Services.Domain.TrackSession
{
    public interface ITrackSessionService
    {
        Task TrackSession(Guid userId, Guid? courseId, Guid? schoolId, CancellationToken cancellationToken);

        Task PageView(Guid userId, string pagePath, Guid? courseId, Guid? schoolId,
            CancellationToken cancellationToken);
    }
}