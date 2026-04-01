using System.ComponentModel;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;
using BioBrain.ViewModels.Interfaces;
using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Implementation
{
    public class ElementsTablePageViewModel : INotifyPropertyChanged, IElemetsTablePageViewModel
    {
        public ElementsTablePageViewModel()
        {

        }

        public StackOrientation Orientation { get; private set; }

        public void SetOrienatation(StackOrientation orientation)
        {
            Orientation = orientation;
            OnPropertyChanged(nameof(Orientation));
        }

        #region PropertyChanged implemetation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // PropertyChanged implemetation
    }
}