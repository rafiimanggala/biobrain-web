using BioBrain.AppResources;
using Microsoft.Maui.Controls;

namespace BioBrain.Helpers
{
    public enum FileTypes
    {
        Material,
        Question,
        Glossary,
        Answer
    }

    public static class FileHelper
    {
        public static string WriteFile(string text, FileTypes fileType)
        {
            string fileName;
            switch (fileType)
            {
                case FileTypes.Question:
                    fileName = "m.html";
                    break;
                case FileTypes.Material:
                    fileName = "m.html";
                    break;
                case FileTypes.Glossary:
                    fileName = "g.html";
                    break;
                case FileTypes.Answer:
                    fileName = "a.html";
                    break;
                default:
                    fileName = "d.html";
                    break;
            }
            var fileWorker = DependencyService.Get<IWorkingWithFiles>();
            return "file://" + fileWorker.CreateHtmlFile(fileName, text);
        }
    }
}