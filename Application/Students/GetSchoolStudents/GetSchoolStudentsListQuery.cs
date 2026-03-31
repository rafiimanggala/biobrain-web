using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Students.GetSchoolStudents
{
    [PublicAPI]
    public sealed class GetSchoolStudentsListQuery : IQuery<List<GetSchoolStudentsListQuery.Result>>
    {
        public Guid SchoolId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid StudentId { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Email { get; set; }
            public Guid? SchoolId { get; init; }
            public List<Guid> SchoolClassIds { get; init; }
            public List<string> SchoolClassNames { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolStudentsListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }

            protected override bool CanExecute(GetSchoolStudentsListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsSchoolAdmin(request.SchoolId) || user.IsSchoolTeacher(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolStudentsListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetSchoolStudentsListQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
                         .Where(StudentSpec.ForSchool(request.SchoolId))
                         .Include(_ => _.SchoolClasses).ThenInclude(_ => _.SchoolClass)
                         .Include(x => x.User)
                         .Select(_ => new Result
                                      {
                                          StudentId = _.StudentId,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          //SchoolId = _.SchoolId,
                                          SchoolClassIds = _.SchoolClasses.Where(sc => sc.SchoolClass.SchoolId == request.SchoolId).Select(sc => sc.SchoolClassId).ToList(),
                                          SchoolClassNames = _.SchoolClasses.Where(sc => sc.SchoolClass.SchoolId == request.SchoolId).Select(sc => sc.SchoolClass.Name).ToList(),
                                          Email = _.User.Email
                         })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
