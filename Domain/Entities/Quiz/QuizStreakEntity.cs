using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;
using Biobrain.Domain.Entities.Student;

namespace Biobrain.Domain.Entities.Quiz
{
	public class QuizStreakEntity : ICreatedEntity, IUpdatedEntity
	{
		public Guid QuizStreakId { get; set; }

		public Guid StudentId { get; set; }
		public StudentEntity Student { get; set; }

		public Guid CourseId { get; set; }
		public CourseEntity Course { get; set; }

		public int WeeksStreak { get; set; }
		public int DaysCount { get; set; }
		public DateTime UpdatedAtLocal { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}