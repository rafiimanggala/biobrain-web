using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class VersionModel : IVersionModel
    {
        public int DataVersion { get; set; }
        public int StructureVersion { get; set; }
        public string Key { get; set; }
    }
}