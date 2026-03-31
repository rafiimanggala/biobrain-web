using System;
using System.Collections.Generic;
using Biobrain.Domain.Constants;
using Biobrain.Domain.Entities.Content;
using Biobrain.Domain.Entities.Templates;

namespace Biobrain.Domain.Entities.Course
{
    public class CourseEntity
    {
        public Guid CourseId { get; set; }
        public string SubHeader { get; set; }
        public string Postfix { get; set; }
        public int SubjectCode { get; set; }
        public int CurriculumCode { get; set; }
		public int Year { get; set; }
        public bool IsForSell { get; set; }
        public bool IsBase { get; set; }
        public CourseGroup Group { get; set; }
        public DateTime LastContentUpdateUtc { get; set; }

        public SubjectEntity Subject { get; set; }
        public CurriculumEntity Curriculum { get; set; }
        public ContentVersionEntity Version { get; set; }
        public List<CourseTemplateEntity> Templates { get; set; }
    }
}
