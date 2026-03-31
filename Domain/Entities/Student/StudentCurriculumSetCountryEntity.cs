using System;

namespace Biobrain.Domain.Entities.Student
{
    public class StudentCurriculumSetCountryEntity
    {
        public Guid StudentCurriculumSetId { get; set; }
        public StudentCurriculumSetEntity StudentCurriculumSet { get; set; }

        public string Country { get; set; }
        public string[]? States { get; set; }
    }
}