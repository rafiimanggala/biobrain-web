using System;
using Biobrain.Domain.Base;
using Biobrain.Domain.Constants;

namespace Biobrain.Domain.Entities.History
{
    public class TempHistoryEntity : ICreatedEntity
    {
        public Guid TempHistoryId { get; set; }
        public Constant.HistoryType Type { get; set; }
        public int DaysAlive { get; set; }
        public Guid RefId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}