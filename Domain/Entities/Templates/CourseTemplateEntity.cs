using System;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.Templates
{
	public class CourseTemplateEntity
	{
		public Guid CourseTemplateId { get; set; }
		public Guid TemplateId { get; set; }
		public TemplateEntity Template { get; set; }
		public Guid CourseId { get; set; }
		public CourseEntity Course { get; set; }
	}
}