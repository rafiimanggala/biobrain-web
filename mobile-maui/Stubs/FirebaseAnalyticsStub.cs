// Firebase Analytics bridge using Plugin.Firebase.Analytics (cross-platform).
// This replaces the previous stub implementations with real Firebase calls.
// The IAnalyticTracker interface is implemented by the platform-specific classes
// in Platforms/iOS and Platforms/Android, which now delegate to Plugin.Firebase.

using System.Collections.Generic;
using Plugin.Firebase.Analytics;

namespace BioBrain.Analytics
{
    /// <summary>
    /// Cross-platform Firebase Analytics tracker using Plugin.Firebase.Analytics.
    /// Replaces both AnalyticTrackerIos and AnalyticTrackerDroid with a single implementation.
    /// </summary>
    public class FirebaseAnalyticsTracker : Interfaces.IAnalyticTracker
    {
        public void SetView(string screenName, string className)
        {
            CrossFirebaseAnalytics.Current.LogEvent("screen_view", new Dictionary<string, object>
            {
                { "screen_name", screenName },
                { "screen_class", className }
            });
        }

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
    }
}
