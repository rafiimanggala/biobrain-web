using DAL.Models.Interfaces;
using SQLite;

namespace DAL.Models.Implementations
{
    [Table("Topics")]
    public class TopicModel : ITopicModel
    {
        [PrimaryKey]
        public int TopicID { get; set; }

        public string TopicName { get; set; }

        public int AreaID { get; set; }

        public string Image { get; set; }

        public string BackgroundImage { get; set; }

        public string DarkImage { get; set; }

        public int TopicOrder { get; set; }
    }
}
