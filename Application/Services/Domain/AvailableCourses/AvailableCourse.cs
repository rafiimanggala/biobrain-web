using System;
using JetBrains.Annotations;

namespace Biobrain.Application.Services.Domain.AvailableCourses
{
    public class AvailableCourse
    {
        public Guid CourseId { get; set; }
        public Guid? ClassId { get; set; }
        public int? ClassYear { get; set; }
        [CanBeNull] public string? ClassName { get; set; }
        public Guid? SchoolId { get; set; }
        [CanBeNull] public string SchoolName { get; set; }
    }
}