using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class FirebaseFileModel : IFirebaseFileModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }
        public string Url { get; set; }
    }
}