namespace DAL.Models.Interfaces
{
    public interface ILevelTypeModel
    {
        int LevelTypeID { get; set; }

        string LevelName { get; set; }

        string LevelShortName { get; set; }

        string Image { get; set; }
    }
}