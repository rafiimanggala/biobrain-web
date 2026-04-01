using System;
using BioBrain.Extensions;
using Microsoft.Maui.Controls;

namespace BioBrain.Views
{
    public partial class HistoryView
    {
        public HistoryView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void HomeButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.MoveToHome();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Navigation.MoveToAbout();
        }
    }
}
