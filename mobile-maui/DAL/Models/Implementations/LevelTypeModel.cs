using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("LevelTypes")]
    public class LevelTypeModel : ILevelTypeModel
    {
        [PrimaryKey]
        public int LevelTypeID { get; set; }

        public string LevelName { get; set; }

        public string LevelShortName { get; set; }

        public string Image { get; set; }
    }
}
