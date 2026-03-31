using System;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.AccessCodes
{
    public class AccessCodeBatchCourseEntity
    {
        public Guid AccessCodeCourseId { get; set; }

        public Guid AccessCodeBatchId { get; set; }
        public AccessCodeBatchEntity AccessCodeBatch { get; set; }

        public Guid CourseId { get; set; }
        public CourseEntity Course { get; set; }
    }
}