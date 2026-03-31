using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Interfaces.ExecutionContext;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.GetSchoolClassStudents
{
    [PublicAPI]
    public sealed class GetSchoolClassStudentsListQuery : IQuery<List<GetSchoolClassStudentsListQuery.Result>>
    {
	    public Guid SchoolId { get; init; }
        public Guid SchoolClassId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid StudentId { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Email { get; init; }
            public Guid? SchoolId { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolClassStudentsListQuery>
        {
	        private readonly IDb _db;
	        private readonly ISessionContext _sessionContext;

	        public PermissionCheck(ISecurityService securityService, IDb db, ISessionContext sessionContext) : base(securityService)
	        {
		        _db = db;
                _sessionContext = sessionContext;
	        }

            protected override bool CanExecute(GetSchoolClassStudentsListQuery request, IUserSecurityInfo user)
            {
                var classTeachers = _db.Teachers.Include(_ => _.Classes)
	                .Where(TeacherSpec.ForClass(request.SchoolClassId)).Select(_ => _.TeacherId).ToList();

                return user.IsApplicationAdmin() || user.IsSchoolAdmin(request.SchoolId) || classTeachers.Contains(_sessionContext.GetUserId());
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolClassStudentsListQuery, List<Result>>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<List<Result>> Handle(GetSchoolClassStudentsListQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
						 .Include(_ => _.SchoolClasses)
						 .Include(x => x.User)
                         .Where(StudentSpec.ForSchool(request.SchoolId))
                         .Where(StudentSpec.ForClass(request.SchoolClassId))
                         .Select(_ => new Result
                                      {
                                          StudentId = _.StudentId,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          Email =  _.User.Email,
                                          //SchoolId = _.SchoolId,
                         })
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);
            }
        }
    }
}
