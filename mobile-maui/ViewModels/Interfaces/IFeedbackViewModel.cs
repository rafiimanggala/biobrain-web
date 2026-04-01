using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IFeedbackViewModel
    {
        Task<string> SendLog();
    }
}