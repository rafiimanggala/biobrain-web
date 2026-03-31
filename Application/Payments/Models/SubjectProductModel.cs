using System.Collections.Generic;

namespace Biobrain.Application.Payments.Models
{
    public class SubjectProductModel
    {
        public int SubjectCode { get; set; }
        public List<CourseProductModel> Courses { get; set; }
    }
}