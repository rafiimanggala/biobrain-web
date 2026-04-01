using BioBrain.ViewModels.Interfaces;
using Common.Interfaces;

namespace BioBrain.Factories.Interfaces
{
    public interface IStatEntryFactory
    {
        IStatEntryViewModel Get(string topic, string level, string date, string score, int topicId, int materialId);
    }
}