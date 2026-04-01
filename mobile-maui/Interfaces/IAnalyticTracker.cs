using System.Collections.Generic;

namespace BioBrain.Interfaces
{
    public interface IAnalyticTracker
    {
        void SetView(string screenName, string className);
        void SendEvent(string eventId);
        void SendEvent(string eventId, string paramName, string value);
        void SendEvent(string eventId, IDictionary<string, string> parameters);
    }
}