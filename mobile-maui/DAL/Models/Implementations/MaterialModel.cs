using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Materials")]
    public class MaterialModel : IMaterialModel
    {
        [PrimaryKey]
        public int MaterialID { get; set; }
        public int TopicID { get; set; }
        public int LevelTypeID { get; set; }
        public string Text { get; set; }
    }
}