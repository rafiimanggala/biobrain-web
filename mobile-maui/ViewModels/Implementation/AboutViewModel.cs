using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.AppResources;
using BioBrain.ViewModels.Interfaces;
using DAL.Repositorys.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class AboutViewModel : IAboutViewModel, INotifyPropertyChanged
    {
        private readonly IDatabaseRepository databaseRepository;
        private string date = string.Empty;
        public AboutViewModel(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        public void GetData()
        {
            date = databaseRepository.GetDatabaseDate();
            OnPropertyChanged(nameof(Date));
        }

        public string FilePath => Path.Combine("file://" + DependencyService.Get<IFilesPath>().PagesPath, "About.html");
        public string Date => $"Content as of {date}";

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}