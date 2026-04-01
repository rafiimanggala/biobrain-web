using Common.Enums;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BioBrain.ViewModels.Interfaces
{
    public interface IDeleteAccountViewModel: INotifyPropertyChanged
    {
        event EventHandler<string> Error;
        event EventHandler FinishDelete;
        DeleteAccountStage Step { get; }
        string Reason { get; set; }
        bool IsReasonVisible { get; }
        void GoToReason();
        Task Submit();
        void Finish();
    }
}