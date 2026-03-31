using System;
using System.Linq;
using Biobrain.Domain.Entities.Teacher;

namespace Biobrain.Application.Extensions
{
	public static class TeacherExtensions
	{
		public static bool IsInSchool(this TeacherEntity teacher, Guid schoolId)
		{
			return teacher.Schools.Any(x => x.SchoolId == schoolId);
		}
	}
}