using System;
using BioBrain.Extensions;
using Common;
using Common.Enums;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.Controls
{
    public class NavigatingEventArgs
    {
        public NavigatingEventArgs(MenuItemsEnum menuItem, bool cancel)
        {
            MenuItem = menuItem;
            Cancel = cancel;
        }

        public MenuItemsEnum MenuItem { get; }
        public bool Cancel { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomBar: IDisposable
    {
        public event EventHandler<NavigatingEventArgs> Navigating;

        public BottomBar()
        {
            InitializeComponent();
            Settings.DataUpdateChanged += SettingsOnDataUpdateChanged;
        }

        private void SettingsOnDataUpdateChanged(object sender, bool e)
        {
	        UpdateBadge.IsVisible = e;
        }

        public static BindableProperty CurrentMenyItemProperty =
            BindableProperty.Create(nameof(CurrentMenyItem), typeof(MenuItemsEnum), typeof(BottomBar), MenuItemsEnum.Home, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (BottomBar)bindable;
                    ctrl.CurrentMenyItem = (MenuItemsEnum)newValue;
                });

        public MenuItemsEnum CurrentMenyItem
        {
            get { return (MenuItemsEnum)GetValue(CurrentMenyItemProperty); }
            set
            {
                SetValue(CurrentMenyItemProperty, value);
            }
        }

        public static BindableProperty IsPeriodicTableVisibleProperty =
            BindableProperty.Create(nameof(IsPeriodicTableVisible), typeof(bool), typeof(BottomBar), false, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (BottomBar)bindable;
                    ctrl.IsPeriodicTableVisible = (bool)newValue;
                });

        public bool IsPeriodicTableVisible
        {
            get { return (bool)GetValue(IsPeriodicTableVisibleProperty); }
            set
            {
                SetValue(IsPeriodicTableVisibleProperty, value);
            }
        }

        private async void Home_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var result = OnNavigating(MenuItemsEnum.Home);
                if(!result.Cancel)
                    await Navigation.MoveToHome();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar Home_OnTapped error: {ex.Message}");
            }
        }

        private async void Glossary_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var result = OnNavigating(MenuItemsEnum.Glossary);
                if (!result.Cancel)
                    await Navigation.MoveToGlossary();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar Glossary_OnTapped error: {ex.Message}");
            }
        }

        private async void PeriodicTabler_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var result = OnNavigating(MenuItemsEnum.PeriodicTable);
                if (!result.Cancel)
                    await Navigation.MoveToPeriodicTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar PeriodicTabler_OnTapped error: {ex.Message}");
            }
        }

        private async void Stats_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var result = OnNavigating(MenuItemsEnum.Stats);
                if (!result.Cancel)
                    await Navigation.MoveToStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar Stats_OnTapped error: {ex.Message}");
            }
        }

        private async void More_OnTapped(object sender, EventArgs e)
        {
            try
            {
                var result = OnNavigating(MenuItemsEnum.More);
                if (!result.Cancel)
                    await Navigation.MoveToAboutBiobrain();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar More_OnTapped error: {ex.Message}");
            }
        }

        protected virtual NavigatingEventArgs OnNavigating(MenuItemsEnum e)
        {
            var args = new NavigatingEventArgs(e, false);
            Navigating?.Invoke(this, args);
            return args;
        }

        public void Dispose()
        {
	        Settings.DataUpdateChanged -= SettingsOnDataUpdateChanged;
        }
    }
}
