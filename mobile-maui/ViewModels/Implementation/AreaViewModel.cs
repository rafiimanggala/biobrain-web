using System.IO;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using DAL.Models.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class AreaViewModel : IAreaViewModel, ICellViewModel
    {
        private readonly IAreaModel model;

        public AreaViewModel(IAreaModel model)
        {
            this.model = model;
        }

        public string Name => model.AreaName.ToUpper();
        public int Id => model.AreaID;
        public string IconPath => Path.Combine(DependencyService.Get<IFilesPath>().ImagesPath, model.Image);
        public bool IsDone { get; set; }
        public string CompletionString { get; set; }
        public bool IsComingSoon => model.IsComingSoon;
    }
}
