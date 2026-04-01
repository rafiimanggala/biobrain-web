using System;
using BioBrain.AppResources;
using BioBrain.Controls;
using BioBrain.Extensions;
using BioBrain.Helpers;
using BioBrain.Interfaces;
using BioBrain.ViewModels.Interfaces;
using Common;
using Common.Enums;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class TextView
    {
        private bool isPostback;
        private readonly IAnalyticTracker analyticTracker = DependencyService.Get<IAnalyticTracker>();

        public ITextViewModel ViewModel
        {
            get => (ITextViewModel)BindingContext;
            set => BindingContext = value;
        }

        public TextView(ITextViewModel viewModel) : base(MenuItemsEnum.More)
        {
            analyticTracker.SetView("Policy Page", nameof(TextView));
            InitializeComponent();
            ViewModel = viewModel;
            //AboutWebView.Source = new UrlWebViewSource {Url = ViewModel.FilePath}; //new HtmlWebViewSource {Html = $@"<center><div style=""color:#004876;font-size:15pt;"">Version: {ViewModel.Version}</div></center>" };
        }

        private async void AboutWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (isPostback)
            {
                if(e.Url.Contains("#")) return;
                e.Cancel = true;
                
                var response = ResponseHelper.GetResponseTextUrls(e.Url);

                switch (response)
                {
                    case "privacy.policy":
                        await Navigation.MoveToTextView(FileNames.PrivacyPolicy, StringResource.PrivacyPolicyString);
                        break;
                    case "payment.policy":
                        await Navigation.MoveToTextView(FileNames.PaymentPolicyPage, StringResource.PaymentPolicyString);
                        break;
                    case "terms.of.service":
                        await Navigation.MoveToTextView(FileNames.TermsOfService, StringResource.TermsOfServiceString);
                        break;
                    case "account.settings":
                        await Navigation.MoveToAccount();
                        break;
                    case "support":
                        await Launcher.OpenAsync(new Uri($"mailto:{Links.SupportEmail}?subject=Biobrain"));
                        break;
                    case "complaints":
                        await Launcher.OpenAsync(new Uri($"mailto:{Links.ComplaintsEmail}?subject=Biobrain"));
                        break;
                    default:
                        await Launcher.OpenAsync(new Uri(e.Url));
                        break;
                }
            }
            isPostback = true;
        }

        private void BottomBar_OnNavigating(object sender, NavigatingEventArgs e)
        {
            Popup.ClosePopup();
        }

        private void TopBaner_OnShare(object sender, EventArgs e)
        {
            if (Popup.IsVisible)
            {
                Popup.ClosePopup();
            }
            else
            {
                Popup.InitPopup();
            }
        }
    }
}
