namespace BioBrain.ViewModels.Interfaces
{
    public interface IStatsViewModel
    {
        string ViewHeader { get; }
        void ToggleSendMode();
        void GetData();
    }
}