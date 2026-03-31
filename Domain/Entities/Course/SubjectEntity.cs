using System.Collections.Generic;
using Biobrain.Domain.Entities.Glossary;


namespace Biobrain.Domain.Entities.Course
{
    public class SubjectEntity
    {
        public int SubjectCode { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public ICollection<TermEntity> Glossary { get; set; }
    }
}