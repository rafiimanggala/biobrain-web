using BioBrain.Factories.Interfaces;
using BioBrain.ViewModels.Implementation;
using Common.Interfaces;

namespace BioBrain.Factories.Implementation
{
    public class StatEntryFactory : IStatEntryFactory
    {
        public IStatEntryViewModel Get(string topic, string level, string date, string score, int topicId, int materialId)
        {
            return new StatEntryViewModel {Topic = topic, Level = level, Date = date, Score = score, TopicId = topicId, MaterialId = materialId};
        }
    }
}