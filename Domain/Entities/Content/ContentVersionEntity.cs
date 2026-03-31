using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.Content
{
	public class ContentVersionEntity : ICreatedEntity, IUpdatedEntity
	{
		public Guid ContentVersionId { get; set; }
		public Guid CourseId { get; set; }
		public CourseEntity Course { get; set; }
		public long Version { get; set; }
		public string ContentFileName { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}