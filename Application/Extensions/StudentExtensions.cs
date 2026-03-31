using System;
using System.Linq;
using Biobrain.Domain.Entities.Student;

namespace Biobrain.Application.Extensions
{
	public static class StudentExtensions
	{
		public static bool IsInSchool(this StudentEntity student, Guid schoolId)
		{
			return student.Schools.Any(x => x.SchoolId == schoolId);
		}
		public static bool IsInSchoolClass(this StudentEntity student, Guid schoolClassId)
		{
			return student.SchoolClasses.Any(x => x.SchoolClassId == schoolClassId);
		}
	}
}