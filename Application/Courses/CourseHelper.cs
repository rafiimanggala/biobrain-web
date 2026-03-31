using Biobrain.Domain.Entities.Course;

namespace Biobrain.Application.Courses
{
	public static class CourseHelper
	{
		public static string GetCourseName(CourseEntity course) => $"{(course.Year == 10 && course.Curriculum.CurriculumCode != 3 ? "" : (course.Curriculum.Name + " "))}{course.Subject.Name} {(course.Year == 0 || course.Curriculum.CurriculumCode == 3 ? "" : "Year " + course.Year)}{(string.IsNullOrEmpty(course.Postfix) ? "" : " " + course.Postfix)}";

        public static string GetShortCourseName(CourseEntity course) =>
            $"{course.Subject.Name} {(course.Year == 0 || course.Year == 10 ? "" : "Year " + course.Year)}{(string.IsNullOrEmpty(course.Postfix) ? "" : " "+course.Postfix)}";
	}
}