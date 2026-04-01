using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Areas")]
    public class AreaModel : IAreaModel
    {
        [Column("AreaID")]
        [PrimaryKey]
        public int AreaID { get; set; }

        [Column("AreaName")]
        public string AreaName { get; set; }

        public string Image { get; set; }

        public bool IsComingSoon { get; set; }
    }
}
