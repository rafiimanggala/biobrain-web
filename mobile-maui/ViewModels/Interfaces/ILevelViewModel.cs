using CustomControls.Interfaces;

namespace BioBrain.ViewModels.Interfaces
{
    public interface ILevelViewModel : ILevelBarElement
    {
        string ShortName { get; }
        string Image { get; }
        int MaterialID { get; }
        bool IsDone { get; set; }
    }
}