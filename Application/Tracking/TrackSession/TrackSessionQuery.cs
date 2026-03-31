using System;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.TrackSession;
using JetBrains.Annotations;

namespace Biobrain.Application.Tracking.TrackSession
{
    [PublicAPI]
    public sealed class TrackSessionQuery : IQuery<TrackSessionQuery.Result>
    {
        public Guid? SchoolId { get; init; }
        public Guid? CourseId { get; init; }


        [PublicAPI]
        public record Result
        {
        }


        internal sealed class PermissionCheck : PermissionCheckBase<TrackSessionQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(TrackSessionQuery request, IUserSecurityInfo user) => true;
        }


        internal sealed class Handler : QueryHandlerBase<TrackSessionQuery, Result>
        {
            private readonly ISessionContext _context;
            private readonly ITrackSessionService _trackSessionService;

            public Handler(IDb db, ISessionContext context, ITrackSessionService trackSessionService) : base(db)
            {
                _context = context;
                _trackSessionService = trackSessionService;
            }

            public override async Task<Result> Handle(TrackSessionQuery request, CancellationToken cancellationToken)
            {
                var userId = _context.GetUserId();
                await _trackSessionService.TrackSession(userId, request.CourseId, request.SchoolId, cancellationToken);
                return new();
            }
        }
    }
}
