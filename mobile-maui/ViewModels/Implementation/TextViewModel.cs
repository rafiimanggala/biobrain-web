using System.IO;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class TextViewModel : ITextViewModel
    {
        public string FileName { get; }
        public string PageHeader { get; }
        public string FilePath => Path.Combine("file://" + DependencyService.Get<IFilesPath>().PagesPath, FileName);
        public string Version => "0.1.11";

        public TextViewModel(string fileName, string pageHeader)
        {
            PageHeader = pageHeader;
            FileName = fileName;
        }
    }
}