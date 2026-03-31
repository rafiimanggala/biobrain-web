using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Biobrain.Application.SchoolClasses.GetSchoolClassById
{
    [PublicAPI]
    public sealed class GetSchoolClassByIdQuery : IQuery<GetSchoolClassByIdQuery.Result>
    {
        public Guid SchoolClassId { get; init; }


        [PublicAPI]
        public sealed class Result
        {
            public Guid SchoolClassId { get; init; }
            public Guid SchoolId { get; init; }
            public Guid CourseId { get; init; }
            public int Year { get; init; }
            public string Name { get; init; }
            public string AutoJoinClassCode { get; init; }
            public ImmutableList<Guid> TeacherIds { get; init; }
            public ImmutableList<Student> Students { get; init; }
        }

        public sealed class Student
        {
            public Guid StudentId { get; set; }
            public string Email { get; set; }
        }


        internal sealed class PermissionCheck : PermissionCheckBase<GetSchoolClassByIdQuery>
        {
            private readonly IDb _db;

            public PermissionCheck(ISecurityService securityService, IDb db)
                : base(securityService)
                => _db = db;

            protected override bool CanExecute(GetSchoolClassByIdQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin())
                    return true;

                var schoolClass = _db.SchoolClasses.GetSingle(SchoolClassSpec.ById(request.SchoolClassId));
                if (user.IsSchoolAdmin(schoolClass.SchoolId))
                    return true;

				// TODO: perhaps teachers assigned (related to) CLASS should be able to execute this query
				if (user.IsSchoolTeacher(schoolClass.SchoolId))
					return true;

				return false;
            }
        }


        internal sealed class Handler : QueryHandlerBase<GetSchoolClassByIdQuery, Result>
        {
            public Handler(IDb db) : base(db) { }

            public override Task<Result> Handle(GetSchoolClassByIdQuery request, CancellationToken cancellationToken)
            {
                return Db.SchoolClasses
                         .Include(_ => _.Teachers)
                         .Include(_ => _.Students)
                         .ThenInclude(x => x.Student)
                         .ThenInclude(x => x.User)
                         .Where(SchoolClassSpec.ById(request.SchoolClassId))
                         .Select(_ => new Result
                                      {
                                          SchoolClassId = _.SchoolClassId,
                                          SchoolId = _.SchoolId,
                                          CourseId = _.CourseId,
                                          Year = _.Year,
                                          Name = _.Name,
                                          AutoJoinClassCode = _.AutoJoinClassCode,
                                          TeacherIds = _.Teachers.Select(x => x.TeacherId).ToList().ToImmutableList(),
                                          Students = _.Students.Select(x => new Student{Email = x.Student.User.Email, StudentId = x.StudentId}).ToList().ToImmutableList()
                         })
                         .AsNoTracking()
                         .GetSingleAsync(cancellationToken);
            }
        }
    }
}
