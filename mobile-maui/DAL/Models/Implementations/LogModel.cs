using Common.Enums;
using DAL.Models.Interfaces;

namespace DAL.Models.Implementations
{
    public class LogModel : ILogModel
    {
        public string Log { get; set; }
        public string Code { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }
        public string User { get; set; }
    }
}