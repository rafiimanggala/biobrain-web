using Common.Enums;

namespace DAL.Models.Interfaces
{
    public interface ILogModel
    {
        string Log { get; set; }
        string Code { get; set; }
        string Version { get; set; }
    }
}