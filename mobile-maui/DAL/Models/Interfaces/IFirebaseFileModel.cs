namespace DAL.Models.Interfaces
{
    public interface IFirebaseFileModel
    {
        string Name { get; set; }
        string Path { get; set; }
        int Size { get; set; }
        string Url { get; set; }
    }
}