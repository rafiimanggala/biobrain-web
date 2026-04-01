namespace DAL.Models.Interfaces
{
    public interface IAreaModel
    {
        int AreaID { get; set; }

        string AreaName { get; set; }

        string Image { get; set; }

        bool IsComingSoon { get; set; }
    }
}