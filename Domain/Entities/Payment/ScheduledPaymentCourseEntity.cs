using System;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.Payment
{
	public class ScheduledPaymentCourseEntity
	{
		public Guid ScheduledPaymentCourseId { get; set; }

		public ScheduledPaymentCourseStatus Status { get; set; }

		public Guid ScheduledPaymentId { get; set; }
		public ScheduledPaymentEntity ScheduledPayment { get; set; }

		public Guid CourseId { get; set; }
		public CourseEntity Course { get; set; }
	}
}