namespace DAL.Models.Interfaces
{
    public interface IMaterialModel
    {
        int MaterialID { get; set; }

        int TopicID { get; set; }

        int LevelTypeID { get; set; }

        string Text { get; set; }
    }
}