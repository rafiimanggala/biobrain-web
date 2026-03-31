using System;

namespace Biobrain.Application.Payments.Models
{
    public class CourseProductModel
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public bool IsComingSoon { get; set; }
    }
}