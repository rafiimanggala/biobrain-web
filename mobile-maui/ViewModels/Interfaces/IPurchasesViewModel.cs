using System;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IPurchasesViewModel
    {
        event EventHandler<string> Error;

        Task GetData();
    }
}