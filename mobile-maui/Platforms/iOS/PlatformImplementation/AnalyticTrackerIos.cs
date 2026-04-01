using System.Collections.Generic;
using BioBrain.Interfaces;
using Plugin.Firebase.Analytics;

namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    /// <summary>
    /// iOS analytics tracker using Plugin.Firebase.Analytics.
    /// Kept for backward compatibility with platform-specific DI registration.
    /// Delegates to the cross-platform Plugin.Firebase.Analytics API.
    /// </summary>
    public class AnalyticTrackerIos : IAnalyticTracker
    {
        public void SendEvent(string eventId)
        {
            CrossFirebaseAnalytics.Current.LogEvent(eventId);
        }

        public void SendEvent(string eventId, string paramName, string value)
        {
            var parameters = new Dictionary<string, object>
            {
                { paramName, value }
            };
            CrossFirebaseAnalytics.Current.LogEvent(eventId, parameters);
        }

        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                CrossFirebaseAnalytics.Current.LogEvent(eventId);
                return;
            }

            var firebaseParams = new Dictionary<string, object>();
            foreach (var kvp in parameters)
            {
                firebaseParams[kvp.Key] = kvp.Value;
            }

            CrossFirebaseAnalytics.Current.LogEvent(eventId, firebaseParams);
        }

        public void SetView(string screenName, string className)
        {
            CrossFirebaseAnalytics.Current.LogEvent("screen_view", new Dictionary<string, object>
            {
                { "screen_name", screenName },
                { "screen_class", className }
            });
        }
    }
}
