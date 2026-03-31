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

namespace Biobrain.Application.Content.GetCourses
{
    [PublicAPI]
    public sealed class GetCoursesQuery : IQuery<List<GetCoursesQuery.Result>>
    {

	    [PublicAPI]
        public record Result
        {
            public Guid CourseId { get; init; }
            public long SubjectCode { get; set; }
            public string Name { get; init; }
            public string SubHeader { get; init; }
            public bool IsBase { get; init; }
            public bool IsGeneric { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetCoursesQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetCoursesQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin();
        }


        internal sealed class Handler : QueryHandlerBase<GetCoursesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override async Task<List<Result>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
            {
                var result = await Db.Courses
	                .Include(x => x.Curriculum)
	                .Include(x => x.Subject)
                         .Select(_ => new Result
                                      {
                                          CourseId = _.CourseId,
                                          Name = CourseHelper.GetCourseName(_),
                                          IsGeneric = _.Curriculum.IsGeneric,
                                          IsBase = _.IsBase,
                                          SubjectCode = _.SubjectCode,
                                          SubHeader = _.SubHeader
                         })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
                result = result.OrderByDescending(_ => _.IsGeneric).ThenByDescending(_ => _.IsBase).ThenBy(_ => _.SubjectCode).ThenBy(_ => _.Name).ToList();
                return result;
            }
        }
    }
}