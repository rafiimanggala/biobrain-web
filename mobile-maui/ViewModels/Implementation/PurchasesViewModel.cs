//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using BioBrain.Annotations;
//using BioBrain.ViewModels.Interfaces;
//using Common;
//using Common.ErrorHandling;
//using Common.Interfaces;
//// using Plugin.Connectivity; // Use Microsoft.Maui.Networking.Connectivity
//using Microsoft.Maui.Controls;

//namespace BioBrain.ViewModels.Implementation
//{
//    public class PurchasesViewModel : IPurchasesViewModel, INotifyPropertyChanged
//    {
//        readonly IErrorLog loger = DependencyService.Get<IErrorLog>();
//        private bool isBusy = true;

//        public event EventHandler<string> Error;

//        public PurchasesViewModel()
//        {
//            Purchases = new ObservableCollection<IPurchaseInfoViewModel>();
//        }

//        public bool IsBusy {
//            get { return isBusy; }
//            set
//            {
//                isBusy = value;
//                OnPropertyChanged(nameof(IsBusy));
//            }
//        }

//        public ObservableCollection<IPurchaseInfoViewModel> Purchases { get; private set; }

//        public async Task GetData()
//        {
//            if (!CrossConnectivity.Current.IsConnected)
//            {
//                OnError(StringResource.NoInternetError);
//                return;
//            }
//            IsBusy = true;
//            try
//            {
//                //Purchases.Clear();
//                //await firebaseService.AuthorizeUser();
//                //var availablePurchases = await firebaseService.GetAvailablePurchases();
//                //availablePurchases.ForEach(p=>Purchases.Add(p));

//            }
//            catch (NoEmailException)
//            {
//                OnError(StringResource.SelectEmailString);
//            }
//            catch (Exception e)
//            {
//                loger.Log(e.Message);
//                //ProcessLabel = string.Empty;
//                Debug.WriteLine(e.InnerException);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        protected virtual void OnError(string e)
//        {
//            Error?.Invoke(this, e);
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        [NotifyPropertyChangedInvocator]
//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}