namespace BioBrain.ViewModels.Interfaces
{
    public interface ITextViewModel
    {
        string FileName { get; }
        string PageHeader { get; }
        string FilePath { get; }
        string Version { get; }
    }
}