using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Courses.GetCoursesForSchool
{
    [PublicAPI]
    public class GetCoursesForSchoolQuery : ICommand<List<GetCoursesForSchoolQuery.Result>>
    {
        public Guid SchoolId { get; set; }

        [PublicAPI]
        public class Result
        {
            public Guid CourseId { get; set; }
            public string Name { get; set; }
            public int SubjectCode { get; set; }
            public int CurriculumCode { get; set; }
            public int Year { get; set; }
        }


        internal class Validator : ValidatorBase<GetCoursesForSchoolQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.SchoolId).ExistsInTable(Db.Schools);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetCoursesForSchoolQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetCoursesForSchoolQuery request, IUserSecurityInfo user) => user.HasAccessToSchool(request.SchoolId);
        }


        internal class Handler : CommandHandlerBase<GetCoursesForSchoolQuery, List<Result>>
        {
            public Handler(IDb db) : base(db)
            {
            }

            // TODO: must consider school subscriptions
            public override async Task<List<Result>> Handle(GetCoursesForSchoolQuery request, CancellationToken cancellationToken)
            {
                return await Db.SchoolCourses.AsNoTracking()
                    .Include(_ => _.Course)
                    .ThenInclude(_ => _.Curriculum)
                    .Include(_ => _.Course)
                    .ThenInclude(_ => _.Subject)
                    .Where(_ => _.SchoolId == request.SchoolId)
                               .Select(_ => new Result
                                            {
                                                CourseId = _.CourseId,
                                                CurriculumCode = _.Course.CurriculumCode,
                                                SubjectCode = _.Course.SubjectCode,
                                                Year = _.Course.Year,
                                                Name = CourseHelper.GetShortCourseName(_.Course)
                               })
                               .ToListAsync(cancellationToken);
            }
        }
    }
}
