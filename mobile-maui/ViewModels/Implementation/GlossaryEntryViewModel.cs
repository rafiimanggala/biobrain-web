using System.IO;
using BioBrain.AppResources;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class GlossaryEntryViewModel
    {
        public GlossaryEntryViewModel(string termin, string eaplanation)
        {
            Termin = (termin+":").ToUpper();
            Explanation = eaplanation;
            WriteData();
        }

        private void WriteData()
        {
            const string fileName = "g.html";
            var paths = DependencyService.Get<IFilesPath>();
            var fileWorker = DependencyService.Get<IWorkingWithFiles>();
            fileWorker.CreateHtmlFile(fileName, HtmlText);
            FilePath = Path.Combine("file://" + paths.PagesPath, fileName);
        }

        public string Termin { get; }

        public string Explanation { get; }

        public string HtmlText => CustomCssStyles.GlossaryPopupStyle + Explanation.Replace("<html>", $"<html><span class=\"termin\">{Termin}</span>");

        public string FilePath { get; set; }
    }
}