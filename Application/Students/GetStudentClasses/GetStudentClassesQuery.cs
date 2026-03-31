using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Students.GetStudentClasses
{
    [PublicAPI]
    public class GetStudentClassesQuery : ICommand<List<GetStudentClassesQuery.Result>>
    {
        public Guid StudentId { get; set; }
        public Guid SchoolId { get; set; }


        [PublicAPI]
        public class Result
        {
            public Guid SchoolClassId { get; set; }
            public int SchoolClassYear { get; set; }
            public string SchoolClassName { get; set; }
            public Guid CourseId { get; set; }
            public int SubjectCode { get; set; }
            public int CurriculumCode { get; set; }
        }


        internal class Validator : ValidatorBase<GetStudentClassesQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetStudentClassesQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GetStudentClassesQuery request, IUserSecurityInfo user)
            {
                if (user.IsApplicationAdmin()) return true;

                var student = _db.Students.Include(x => x.Schools).GetSingle(StudentSpec.ById(request.StudentId));
                if (user.IsAccountOwner(student.StudentId)) return true;
                return student.Schools.Any(x => user.IsSchoolAdmin(x.SchoolId) || user.IsSchoolTeacher(x.SchoolId));
            }
        }


        internal class Handler : CommandHandlerBase<GetStudentClassesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<Result>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
            {
                return await Db.Students
                               .Include(_ => _.SchoolClasses).ThenInclude(_ => _.SchoolClass)
                               .Where(StudentSpec.ById(request.StudentId))
                               .SelectMany(_ => _.SchoolClasses)
                               .Where(_ => _.SchoolClass.SchoolId == request.SchoolId)
                               .Select(_ => new Result
                                            {
                                                SchoolClassId = _.SchoolClassId,
                                                SchoolClassYear = _.SchoolClass.Year,
                                                SchoolClassName = _.SchoolClass.Name,
                                                CourseId = _.SchoolClass.CourseId,
                                                SubjectCode = _.SchoolClass.Course.SubjectCode,
                                                CurriculumCode = _.SchoolClass.Course.CurriculumCode
                                            })
                               .ToListAsync(cancellationToken);
            }
        }
    }
}