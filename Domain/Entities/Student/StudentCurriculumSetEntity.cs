using System;
using System.Collections.Generic;
using Biobrain.Domain.Entities.Course;

namespace Biobrain.Domain.Entities.Student
{
    public class StudentCurriculumSetEntity
    {
        public Guid StudentCurriculumSetId { get; set; }
        public string Name { get; set; }

        public int MainCurriculumCode { get; set; }
        public CurriculumEntity Curriculum { get; set; }

        public IEnumerable<StudentCurriculumSetCountryEntity> Countries { get; set; }
        public IEnumerable<StudentCurriculumSetEntryEntity> Courses { get; set; }
    }
}