using System;
using System.Linq;
using Biobrain.Application.Common.Specifications;
using Biobrain.Domain.Entities.School;
using Biobrain.Domain.Entities.Student;
using Microsoft.EntityFrameworkCore;

namespace Biobrain.Application.Common.DbRequests
{
	public static class StudentDbRequests
	{
		public static bool CheckLicenseOverflowForSchool(this DbSet<StudentEntity> students, SchoolEntity school, int studentsToAdd = 1) => students.LicenseUsed(school.SchoolId) + studentsToAdd > school.StudentsLicensesNumber;

        public static int LicenseUsed(this DbSet<StudentEntity> students, Guid schoolId)
		{
			return students
                .Include(x => x.SchoolClasses)
                .ThenInclude(_ => _.SchoolClass)
                .Where(StudentSpec.ForSchool(schoolId))
				.Sum(x => x.SchoolClasses.Count(c => c.SchoolClass.SchoolId == schoolId));
		}
	}
}