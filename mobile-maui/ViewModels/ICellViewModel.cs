namespace BioBrain.ViewModels
{
    public interface ICellViewModel
    {
        string Name { get; }
        string CompletionString { get; }
        string IconPath { get; }
        bool IsDone { get; }
    }
}