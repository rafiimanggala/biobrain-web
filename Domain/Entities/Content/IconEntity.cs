using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobrain.Domain.Entities.Content
{
    [Table("Icons")]
    public class IconEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IconId { get; set; }

        public string Reference { get; set; }

        public long SubjectId { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }
    }
}
