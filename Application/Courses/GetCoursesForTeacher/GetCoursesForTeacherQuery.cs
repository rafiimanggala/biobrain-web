using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services.Domain.AvailableCourses;
using JetBrains.Annotations;

namespace Biobrain.Application.Courses.GetCoursesForTeacher
{
    [PublicAPI]
    public class GetCoursesForTeacherQuery : ICommand<List<GetCoursesForTeacherQuery.Result>>
    {
        public Guid TeacherId { get; set; }

        [PublicAPI]
        public class Result
        {
	        public Guid TeacherId { get; set; }
            public Guid? SchoolId { get; set; }
            public string SchoolName { get; set; }
            public List<Course> Courses { get; set; }
        }

        public class Course
        {
	        public Guid CourseId { get; set; }
	        public Guid ClassId { get; set; }
	        public string ClassName { get; set; }
	        public int ClassYear { get; set; }
        }

        internal class Validator : ValidatorBase<GetCoursesForTeacherQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.TeacherId).ExistsInTable(Db.Teachers);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetCoursesForTeacherQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetCoursesForTeacherQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsTeacherAccountOwner(request.TeacherId);
        }


        internal class Handler : CommandHandlerBase<GetCoursesForTeacherQuery, List<Result>>
        {
	        private readonly IAvailableCoursesService _availableCoursesService;
            public Handler(IDb db, IAvailableCoursesService availableCoursesService) : base(db) => _availableCoursesService = availableCoursesService;

            public override async Task<List<Result>> Handle(GetCoursesForTeacherQuery request, CancellationToken cancellationToken)
            {
	            return (await _availableCoursesService.GetAvailableTeacherClassCourses(request.TeacherId,
			            cancellationToken)).GroupBy(x => new {SchoolId = x.Item3.SchoolId, SchoolName = x.Item2})
		            ?.Select(_ => new Result
		            {
			            TeacherId = request.TeacherId, SchoolId = _.Key.SchoolId, SchoolName = _.Key.SchoolName,
			            Courses = _.Select(c => new Course {ClassName = c.Item3.Name, CourseId = c.Item1.CourseId, ClassId = c.Item3.SchoolClassId, ClassYear = c.Item1.Year }).ToList()
		            }).ToList();
            }
            
           
        }
    }
}
