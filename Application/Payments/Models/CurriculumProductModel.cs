using System.Collections.Generic;

namespace Biobrain.Application.Payments.Models
{
    public class CurriculumProductModel
    {
        public int CurriculumCode { get; set; }
        public List<SubjectProductModel> Subjects { get; set; }
    }
}