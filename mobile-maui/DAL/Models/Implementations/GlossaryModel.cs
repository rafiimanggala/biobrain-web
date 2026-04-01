using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Glossary")]
    public class GlossaryModel : IGlossaryModel
    {
        [PrimaryKey]
        public int TermID { get; set; }

        public string Ref { get; set; }

        public string Term { get; set; }

        public string Definition { get; set; }

        public string Header { get; set; }
    }
}