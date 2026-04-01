using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("DoneAreas")]
    public class DoneAreaModel : IDoneAreaModel
    {
        [PrimaryKey, AutoIncrement]
        public int DoneAreaID { get; set; }
        public int AreaID { get; set; }
    }
}
