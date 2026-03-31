using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.SchoolClass;

namespace Biobrain.Application.Services.Domain.AvailableCourses
{
	public interface IAvailableCoursesService
	{
		Task<List<CourseEntity>> GetAvailableStudentCourses(Guid studentId, CancellationToken cancellationToken);

		/// <summary>
		/// Get available courses {courseId, className, schoolId, schoolName}
		/// </summary>
		/// <param name="studentId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<Tuple<CourseEntity, string, SchoolClassEntity>>> GetAvailableStudentClassCourses(Guid studentId,
			CancellationToken cancellationToken);

		/// <summary>
		/// Get available courses {courseId, schoolName, class}
		/// </summary>
		/// <param name="teacherId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<Tuple<CourseEntity, string, SchoolClassEntity>>> GetAvailableTeacherClassCourses(Guid teacherId,
			CancellationToken cancellationToken);
		Task<List<CourseEntity>> GetAvailableTeacherCourses(Guid teacherId, CancellationToken cancellationToken);
	}
}