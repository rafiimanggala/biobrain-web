using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.SchoolClasses.GetSchoolClasses
{
    [PublicAPI]
    public sealed class GetSchoolClassesListQuery : IQuery<List<GetSchoolClassesListQuery.Result>>
    {
        public Guid SchoolId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid SchoolClassId { get; init; }
            public Guid SchoolId { get; init; }
            public Guid CourseId { get; init; }
            public int Year { get; init; }
            public string Name { get; init; }
            public string AutoJoinClassCode { get; init; }
            public ImmutableList<string> Teachers { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolClassesListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolClassesListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsSchoolAdmin(request.SchoolId) || user.IsSchoolTeacher(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolClassesListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }
            public override Task<List<Result>> Handle(GetSchoolClassesListQuery request, CancellationToken cancellationToken)
            {
                return Db.SchoolClasses
                         .Include(_ => _.Teachers)
                         .ThenInclude(_ => _.Teacher)
                         .Where(SchoolClassSpec.ForSchool(request.SchoolId))
                         .Select(_ => new Result
                                      {
                                          SchoolClassId = _.SchoolClassId,
                                          SchoolId = _.SchoolId,
                                          Year = _.Year,
                                          Name = _.Name,
                                          AutoJoinClassCode = _.AutoJoinClassCode,
                                          CourseId = _.CourseId,
                                          Teachers = _.Teachers
                                                      .Select(t => $"{t.Teacher.FirstName} {t.Teacher.LastName}")
                                                      .ToList()
                                                      .ToImmutableList()
                                      })
                         .OrderBy(_ => _.Year).ThenBy(_ => _.CourseId)
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
