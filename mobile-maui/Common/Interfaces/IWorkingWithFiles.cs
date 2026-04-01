namespace BioBrain.AppResources
{
    public interface IWorkingWithFiles
    {
        string CreateHtmlFile(string fileName, string html);

        string MoveFileToAppDirectory(string filePath);
    }
}