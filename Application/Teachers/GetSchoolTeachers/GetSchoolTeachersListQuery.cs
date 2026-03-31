using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Teachers.GetSchoolTeachers
{
    [PublicAPI]
    public sealed class GetSchoolTeachersListQuery : IQuery<List<GetSchoolTeachersListQuery.Result>>
    {
        public Guid SchoolId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid TeacherId { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public Guid SchoolId { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolTeachersListQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService) { }
            protected override bool CanExecute(GetSchoolTeachersListQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsSchoolAdmin(request.SchoolId);
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolTeachersListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }
            public override Task<List<Result>> Handle(GetSchoolTeachersListQuery request, CancellationToken cancellationToken)
            {
                return Db.Teachers
	                .Include(x => x.Schools)
                         .Where(TeacherSpec.ForSchool(request.SchoolId))
                         .Select(_ => new Result
                                      {
                                          TeacherId = _.TeacherId,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          SchoolId = request.SchoolId
                                      })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
