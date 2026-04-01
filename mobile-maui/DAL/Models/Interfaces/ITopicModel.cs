namespace DAL.Models.Interfaces
{
    public interface ITopicModel
    {
        int TopicID { get; set; }

        string TopicName { get; set; }

        int AreaID { get; set; }

        string Image { get; set; }

        string BackgroundImage { get; set; }

        string DarkImage { get; set; }

        int TopicOrder { get; set; }
    }
}