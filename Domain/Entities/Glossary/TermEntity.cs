using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobrain.Domain.Base;
using Biobrain.Domain.Entities.Course;


namespace Biobrain.Domain.Entities.Glossary
{
    [Table("GlossaryTerms")]
    public class TermEntity : IDeletedEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TermId { get; set; }

        public string Ref { get; set; }

        public string Term { get; set; }

        public string Definition { get; set; }

        public string Header { get; set; }

        public int SubjectCode { get; set; }

        public SubjectEntity Subject { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
