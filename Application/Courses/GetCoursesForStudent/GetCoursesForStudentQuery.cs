using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Core;
using Biobrain.Application.Common.Validators;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Security;
using Biobrain.Application.Services;
using Biobrain.Application.Services.Domain.AvailableCourses;
using JetBrains.Annotations;

namespace Biobrain.Application.Courses.GetCoursesForStudent
{
    [PublicAPI]
    public class GetCoursesForStudentQuery : ICommand<List<GetCoursesForStudentQuery.Result>>
    {
        public Guid StudentId { get; set; }
        public DateTime LocalDateTime { get; set; }

        [PublicAPI]
        public class Result
        {
	        public Guid StudentId { get; set; }
            public Guid? SchoolId { get; set; }
            public string SchoolName { get; set; }
            public List<Course> Courses { get; set; }
        }

        public class Course
        {
	        public Guid CourseId { get; set; }
            public string CourseName { get; set; }
            public Guid? ClassId { get; set; }
            [CanBeNull] public string ClassName { get; set; }
            public int? ClassYear { get; set; }
            public int Streak { get; set; }
        }

        internal class Validator : ValidatorBase<GetCoursesForStudentQuery>
        {
            public Validator(IDb db) : base(db)
            {
                RuleFor(_ => _.StudentId).ExistsInTable(Db.Students);
            }
        }


        internal class PermissionCheck : PermissionCheckBase<GetCoursesForStudentQuery>
        {
            public PermissionCheck(ISecurityService securityService) : base(securityService)
            {
            }

            protected override bool CanExecute(GetCoursesForStudentQuery request, IUserSecurityInfo user) => user.IsApplicationAdmin() || user.IsStudentAccountOwner(request.StudentId);
        }


        internal class Handler : CommandHandlerBase<GetCoursesForStudentQuery, List<Result>>
        {
	        private readonly IAvailableCoursesService _availableCoursesService;
	        private readonly IQuizStreakService _streakService;
            public Handler(IDb db, IAvailableCoursesService availableCoursesService, IQuizStreakService streakService) : base(db)
            {
	            _availableCoursesService = availableCoursesService;
                _streakService = streakService;
            }

            public override async Task<List<Result>> Handle(GetCoursesForStudentQuery request, CancellationToken cancellationToken)
            {
                var courseGroups = (await _availableCoursesService.GetAvailableStudentClassCourses(request.StudentId,
                        cancellationToken)).GroupBy(x => new { SchoolId = x.Item3?.SchoolId, SchoolName = x.Item2 })
                    ?.Select(_ => new Result
                    {
                        StudentId = request.StudentId,
                        SchoolId = _.Key.SchoolId,
                        SchoolName = _.Key.SchoolName,
                        Courses = _.Select(c => new Course { ClassName = c.Item3?.Name, CourseId = c.Item1.CourseId, ClassId = c.Item3?.SchoolClassId, ClassYear = c.Item3?.Year, CourseName = CourseHelper.GetShortCourseName(c.Item1) }).ToList()
                    }).ToList();
                foreach (var group in courseGroups)
                {
                    foreach (var course in group.Courses)
                    {
                        var streak = await _streakService.GetStreak(request.StudentId, course.CourseId, request.LocalDateTime);
                        course.Streak = (int)streak.DaysCount;
                    }
                }

                return courseGroups;
            }
            
           
        }
    }
}
