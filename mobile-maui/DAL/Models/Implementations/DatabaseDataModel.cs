using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("DatabaseData")]
    public class DatabaseDataModel : IDatabaseDataModel
    {
        public string Property { get; set; }
        public string Value { get; set; }
    }
}