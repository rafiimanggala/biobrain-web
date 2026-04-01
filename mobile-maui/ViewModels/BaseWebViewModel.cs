using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BioBrain.Annotations;

namespace BioBrain.ViewModels
{
    public abstract class BaseWebViewModel : IBaseWebViewModel
    {
        public event EventHandler<string> Error; 

        public bool IsBusy { get; private set; } = true;

        public bool IsShowData => !IsBusy && !IsLocked;

        public bool IsLocked { get; private set; }

        protected void StartProcess()
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(IsShowData));
        }

        protected void EndProcess()
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsBusy));
            OnPropertyChanged(nameof(IsShowData));
        }

        /// <summary>
        /// Public version of EndProcess for external callers (e.g., View code-behind).
        /// </summary>
        public void ForceEndProcess() => EndProcess();

        protected void LockApp()
        {
            IsLocked = true;
            OnPropertyChanged(nameof(IsLocked));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnError(string e)
        {
            Error?.Invoke(this, e);
        }
    }
}