using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.Students.GetStudentById
{
    [PublicAPI]
    public sealed class GetStudentByIdQuery : IQuery<GetStudentByIdQuery.Result>
    {
        public Guid StudentId { get; init; }


        [PublicAPI]
        public record Result
        {
            public Guid StudentId { get; init; }
            public string Email { get; set; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Country { get; init; }
            public string State { get; set; }
            public int? CurriculumCode { get; set; }
            public int? Year { get; set; }
            public Guid? SchoolId { get; init; }
            public List<Guid> SchoolClassIds { get; init; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetStudentByIdQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetStudentByIdQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

				var student = _db.Students.Include(x => x.Schools).GetSingle(StudentSpec.ById(request.StudentId));
				if (student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId) || user.IsSchoolTeacher(x.SchoolId)))
					return true;

				return user.IsStudentAccountOwner(request.StudentId);
			}
        }


        internal sealed class Handler : QueryHandlerBase<GetStudentByIdQuery, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<Result> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
            {
                return Db.Students
                         .Where(StudentSpec.ById(request.StudentId))
                         .Include(_ => _.User)
                         .Include(_ => _.SchoolClasses)
                         .Select(_ => new Result
                                      {
                                          StudentId = _.StudentId,
                                          Email = _.User.Email,
                                          FirstName = _.FirstName,
                                          LastName = _.LastName,
                                          Country = _.Country,
                                          State = _.State,
                                          Year = _.Year,
                                          CurriculumCode = _.CurriculumCode,
                                          //SchoolId = _.SchoolId,
                                          SchoolClassIds = _.SchoolClasses.Select(c => c.SchoolClassId).ToList(),
                                      })
                         .AsNoTracking()
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}
