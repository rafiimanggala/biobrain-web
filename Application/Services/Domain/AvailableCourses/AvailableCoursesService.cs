using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Common.Extensions;
using Biobrain.Application.Common.Specifications;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.SchoolClass;
using Biobrain.Domain.Entities.Student;
using Biobrain.Domain.Entities.Teacher;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Services.Domain.AvailableCourses
{
	public class AvailableCoursesService: IAvailableCoursesService
	{
		private readonly IDb _db;

		public AvailableCoursesService(IDb db) => _db = db;

        public async Task<List<CourseEntity>> GetAvailableTeacherCourses(Guid teacherId, CancellationToken cancellationToken)
		{
			return await _db.Teachers
				.Include(_ => _.Classes).ThenInclude(_ => _.SchoolClass).ThenInclude(x => x.Course)
                .ThenInclude(x => x.Subject)
				.Where(TeacherSpec.ById(teacherId))
				.SelectMany(_ => _.Classes)
				.Select(_ => _.SchoolClass.Course)
				.ToListAsync(cancellationToken);
		}

		/// <summary>
		/// Get available courses {courseId, schoolName, class}
		/// </summary>
		/// <param name="teacherId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<List<Tuple<CourseEntity, string, SchoolClassEntity>>> GetAvailableTeacherClassCourses(Guid teacherId,
			CancellationToken cancellationToken)
		{
			var teacher = await _db.Teachers
				.Include(_ => _.Classes).ThenInclude(_ => _.SchoolClass)
				.ThenInclude(x => x.School)
                .Include(_ => _.Classes)
                .ThenInclude(_ => _.SchoolClass)
                .ThenInclude(_ => _.Course)
                .ThenInclude(x => x.Subject)
				.GetSingleAsync(TeacherSpec.ById(teacherId), cancellationToken);
			var courses = new List<Tuple<CourseEntity, string, SchoolClassEntity>>(); //await GetStandaloneStudentCourses(student);
			courses.AddRange(GetSchoolTeacherCourses(teacher));

			return courses;
		}

		/// <summary>
		/// Get available courses {courseId, className, schoolId, schoolName}
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Course, SchoolName, SchoolClass</returns>
		public async Task<List<Tuple<CourseEntity, string, SchoolClassEntity>>> GetAvailableStudentClassCourses(Guid studentId,
			CancellationToken cancellationToken)
		{
			var student = await _db.Students
				.Include(_ => _.SchoolClasses).ThenInclude(_ => _.SchoolClass)
				.ThenInclude(x =>x.School)
                .Include(_ => _.SchoolClasses)
                .ThenInclude(_ => _.SchoolClass)
                .ThenInclude(_ => _.Course)
                .ThenInclude(x => x.Subject)
				.GetSingleAsync(StudentSpec.ById(studentId), cancellationToken);
			var courses = GetSchoolStudentCourses(student);
			courses.AddRange(await GetStandaloneStudentCourses(student, courses.Where(_ => _.Item3.School.UseAccessCodes).ToList()));

			return courses;
		}

		public async Task<List<CourseEntity>> GetAvailableStudentCourses(Guid studentId, CancellationToken cancellationToken)
		{
			return (await GetAvailableStudentClassCourses(studentId, cancellationToken)).Select(_ => _.Item1).ToList();
		}

		private async Task<List<Tuple<CourseEntity, string, SchoolClassEntity>>> GetStandaloneStudentCourses(StudentEntity student, List<Tuple<CourseEntity, string, SchoolClassEntity>> accessCodeSchoolCourses)
		{
			var subscriptions = await _db.ScheduledPayment
				.Include(x => x.ScheduledPaymentCourses)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Subject)

                .Include(x => x.ScheduledPaymentCourses)
                .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Curriculum)


				.Where(_ => _.UserId == student.StudentId && _.DeletedAt == null)
				.Where(x => x.Status == ScheduledPaymentStatus.Success || x.Status == ScheduledPaymentStatus.StoppedByUser)
				.ToListAsync();
            if (subscriptions.Any(_ => _.Type != ScheduledPaymentType.FreeTrial))
            {
                subscriptions.RemoveAll(_ => _.Type == ScheduledPaymentType.FreeTrial);
            }

			var result = new List<CourseEntity>();
            subscriptions
                .ForEach(x => x.ScheduledPaymentCourses
                    .Where(y => y.Status == ScheduledPaymentCourseStatus.Active ||
                                y.Status == ScheduledPaymentCourseStatus.StoppedByUser)
                    .Where(_ => accessCodeSchoolCourses.All(ac => ac.Item1.CourseId != _.CourseId))
                    .ToList().ForEach(c => result.Add(c.Course))
                );

			return result.DistinctBy(_ => _.CourseId).Select(_ => new Tuple<CourseEntity, string, SchoolClassEntity>(_, "", null)).ToList();
		}

		private List<Tuple<CourseEntity, string, SchoolClassEntity>> GetSchoolStudentCourses(StudentEntity student)
		{
			return student.SchoolClasses
				.Where(_ => _.SchoolClass.School.Status == Constant.SchoolStatus.FreeTrial || _.SchoolClass.School.Status == Constant.SchoolStatus.LiveCustomer)
                .Where(_ => _.SchoolClass.School.EndDateUtc == null || _.SchoolClass.School.EndDateUtc > DateTime.UtcNow)
				.Select(_ => new Tuple<CourseEntity, string, SchoolClassEntity>(_.SchoolClass.Course, _.SchoolClass.School.Name, _.SchoolClass))
				.ToList();
		}

		private List<Tuple<CourseEntity, string, SchoolClassEntity>> GetSchoolTeacherCourses(TeacherEntity teacher)
		{
			return teacher.Classes
				.Where(_ => _.SchoolClass.School.Status == Constant.SchoolStatus.FreeTrial || _.SchoolClass.School.Status == Constant.SchoolStatus.LiveCustomer)
                .Where(_ => _.SchoolClass.School.EndDateUtc == null || _.SchoolClass.School.EndDateUtc > DateTime.UtcNow)
				.Select(_ => new Tuple<CourseEntity, string, SchoolClassEntity>(_.SchoolClass.Course, _.SchoolClass.School.Name, _.SchoolClass))
				.ToList();
		}
	}
}