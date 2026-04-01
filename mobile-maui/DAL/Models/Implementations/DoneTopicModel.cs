using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("DoneTopics")]
    public class DoneTopicModel : IDoneTopicModel
    {
        [PrimaryKey, AutoIncrement]
        public int DoneTopicID { get; set; }
        public int TopicID { get; set; }
    }
}
