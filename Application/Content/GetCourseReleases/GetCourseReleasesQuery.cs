using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Courses;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Content.GetCourseReleases
{
    [PublicAPI]
    public sealed class GetCourseReleasesQuery : IQuery<List<GetCourseReleasesQuery.Result>>
    {

	    [PublicAPI]
        public record Result
        {
            public Guid CourseId { get; init; }
            public string Name { get; init; }
            public DateTime LastUploadDateTimeUtc { get; init; }
            public DateTime LastReleaseDateTimeUtc { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetCourseReleasesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetCourseReleasesQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetCourseReleasesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetCourseReleasesQuery request, CancellationToken cancellationToken)
            {
                var result = await Db.Courses
	                .Include(x => x.Curriculum)
	                .Include(x => x.Subject)
                    .Include(x => x.Version)
                         .Select(_ => new Result
                                      {
                                          CourseId = _.CourseId,
                                          Name = CourseHelper.GetCourseName(_),
                                          LastUploadDateTimeUtc = _.LastContentUpdateUtc,
                                          LastReleaseDateTimeUtc = _.Version == null ? DateTime.MinValue : _.Version.UpdatedAt
                         })
                    .AsNoTracking()
                         .ToListAsync(cancellationToken);
                result = result.OrderBy(_ => _.Name).ToList();
                return result;
            }
        }
    }
}