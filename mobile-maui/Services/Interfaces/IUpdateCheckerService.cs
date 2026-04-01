namespace BioBrain.Services.Interfaces
{
    public interface IBackgroundWorkerService
    {
        //void Init(App app);
        void CheckUpdate(object state);
        void StartPeriodically();
        void StopPeriodically();
    }
}