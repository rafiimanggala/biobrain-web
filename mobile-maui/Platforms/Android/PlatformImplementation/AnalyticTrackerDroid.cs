using System.Collections.Generic;
using Android.OS;
using BioBrain.Interfaces;
using Firebase.Analytics;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IAnalyticTracker, AnalyticTrackerDroid>();
namespace BioBrain.Platforms.Android.PlatformImplementation
{
    public class AnalyticTrackerDroid : IAnalyticTracker
    {
        public void SendEvent(string eventId)
        {
            SendEvent(eventId, null);
        }

        public void SendEvent(string eventId, string paramName, string value)
        {
            SendEvent(eventId, new Dictionary<string, string>
            {
                { paramName, value }
            });
        }

        public void SendEvent(string eventId, IDictionary<string, string> parameters)
        {
            // TODO: Replace MainActivity.PrimaryActivity with Platform.CurrentActivity or DI
            var firebaseAnalytics = FirebaseAnalytics.GetInstance(Platform.CurrentActivity);

            if (parameters == null)
            {
                firebaseAnalytics.LogEvent(eventId, null);
                return;
            }

            var bundle = new Bundle();
            foreach (var param in parameters)
            {
                bundle.PutString(param.Key, param.Value);
            }

            firebaseAnalytics.LogEvent(eventId, bundle);
        }

        public void SetView(string screenName, string className)
        {
            var firebaseAnalytics = FirebaseAnalytics.GetInstance(Platform.CurrentActivity);
            firebaseAnalytics.SetCurrentScreen(Platform.CurrentActivity, screenName, className);
        }
    }
}
