using Common;
using Common.Enums;
using CustomControls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace BioBrain.Views
{
    public class BaseView : ContentPage, IBaseView
    {
        public virtual MenuItemsEnum MenuItem { get; }

        public BaseView(MenuItemsEnum menuItem)
        {
            MenuItem = menuItem;
            Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
            On<Microsoft.Maui.Controls.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            BackgroundColor = CustomColors.DarkMain;
        }

        protected async void OnError(object sender, string message)
        {
	        await DisplayAlert(StringResource.ErrorString, message,
		        StringResource.OkString, StringResource.CancelString);
        }
    }
}