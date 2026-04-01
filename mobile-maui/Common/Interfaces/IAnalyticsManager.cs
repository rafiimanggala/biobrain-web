namespace Common.Interfaces
{

    public interface IAnalyticsManager
    {
        IAnalyticsManager InitWithId(string analyticsId);
        void TrackScreen(string screen);
    }
}