namespace BioBrain.ViewModels.Interfaces
{
    public interface IWordViewModel
    {
        int WordID { get; }

        string Term { get; }

        string Defenition { get; }

        string HtmlText { get; }

        string FilePath { get; }

        string PopupFilePath { get; }

        IWordViewModel GetGlossaryTerm(string reference);
    }
}