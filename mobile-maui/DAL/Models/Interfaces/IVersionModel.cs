namespace DAL.Models.Interfaces
{
    public interface IVersionModel
    {
        int DataVersion { get; set; }
        int StructureVersion { get; set; }
        string Key { get; set; }
    }
}