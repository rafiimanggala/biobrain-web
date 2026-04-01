using Microsoft.Maui.Controls;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IElemetsTablePageViewModel
    {
        StackOrientation Orientation { get; }
        void SetOrienatation(StackOrientation orientation);
    }
}