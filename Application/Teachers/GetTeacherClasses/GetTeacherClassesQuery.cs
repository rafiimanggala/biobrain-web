using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Specifications;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Teachers.GetTeacherClasses
{
    [PublicAPI]
    public class GetTeacherClassesQuery : ICommand<List<GetTeacherClassesQuery.Result>>
    {
        public Guid TeacherId { get; set; }
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


        internal class Validator : ValidatorBase<GetTeacherClassesQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetTeacherClassesQuery>
        {
            private readonly IDb _db;
            public PermissionCheck(ISecurityService securityService, IDb db) : base(securityService) => _db = db;

            protected override bool CanExecute(GetTeacherClassesQuery request, IUserSecurityInfo user)
            {
                var teacher = _db.Teachers.Include(x => x.Schools).GetSingle(TeacherSpec.ById(request.TeacherId));
                return teacher.Schools.Any(x => user.HasAccessToSchool(x.SchoolId));
            }
        }


        internal class Handler : CommandHandlerBase<GetTeacherClassesQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            public override async Task<List<Result>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
            {
                var items = await Db.Teachers
                    .Include(_ => _.Classes).ThenInclude(_ => _.SchoolClass)
                    .ThenInclude(_ => _.Course)
                    .Where(TeacherSpec.ById(request.TeacherId))
                    .SelectMany(_ => _.Classes)
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
                return items;
            }
        }
    }
}
