using System;
using BioBrain.Extensions;
using Common.Enums;
using Microsoft.Maui.Controls;

namespace BioBrain.Controls
{
    public partial class TopBaner
    {
        public event EventHandler<NavigatingEventArgs> Navigating;

        public event EventHandler Share;
        public event EventHandler Send;

        public TopBaner()
        {
            InitializeComponent();
        }

        public static BindableProperty TextProperty =
           BindableProperty.Create(nameof(Text), typeof(string), typeof(TopBaner), string.Empty, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (TopBaner)bindable;
                   ctrl.Text = (string)newValue;
               });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                HeaderLabel.Text = value;
                HeaderLabel.FontSize = /*value.Length > 21 ? 16 :*/ 20;
                SetValue(TextProperty, value);
            }
        }

        public static BindableProperty IsBackVisibleProperty =
           BindableProperty.Create(nameof(IsBackVisible), typeof(bool), typeof(TopBaner), true, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (TopBaner)bindable;
                   ctrl.IsBackVisible = (bool)newValue;
               });

        public bool IsBackVisible
        {
            get => (bool)GetValue(IsBackVisibleProperty);
            set
            {
                BackButton.IsVisible = value;
                SetValue(IsBackVisibleProperty, value);
            }
        }

        public static BindableProperty IsLogoVisibleProperty =
           BindableProperty.Create(nameof(IsLogoVisible), typeof(bool), typeof(TopBaner), false, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (TopBaner)bindable;
                   ctrl.IsLogoVisible = (bool)newValue;
               });

        public bool IsLogoVisible
        {
            get => (bool)GetValue(IsLogoVisibleProperty);
            set
            {
                BiobrainImage.IsVisible = value;
                SetValue(IsLogoVisibleProperty, value);
            }
        }

        public static BindableProperty IsSendVisibleProperty =
           BindableProperty.Create(nameof(IsSendVisible), typeof(bool), typeof(TopBaner), false, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (TopBaner)bindable;
                   ctrl.IsSendVisible = (bool)newValue;
               });

        public bool IsSendVisible
        {
            get => (bool)GetValue(IsSendVisibleProperty);
            set
            {
                SendButton.IsVisible = value;
                SetValue(IsSendVisibleProperty, value);
            }
        }

        public static BindableProperty IsRightPlaceholderVisibleProperty =
           BindableProperty.Create(nameof(IsRightPlaceholderVisible), typeof(bool), typeof(TopBaner), false, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (TopBaner)bindable;
                   ctrl.IsRightPlaceholderVisible = (bool)newValue;
               });

        public bool IsRightPlaceholderVisible
        {
            get => (bool)GetValue(IsRightPlaceholderVisibleProperty);
            set
            {
                RightPlaceholder.IsVisible = value;
                SetValue(IsRightPlaceholderVisibleProperty, value);
            }
        }

        public static BindableProperty IsShareVisibleProperty =
            BindableProperty.Create(nameof(IsShareVisible), typeof(bool), typeof(TopBaner), true, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TopBaner)bindable;
                    ctrl.IsShareVisible = (bool)newValue;
                });

        public bool IsShareVisible
        {
            get => (bool)GetValue(IsShareVisibleProperty);
            set
            {
                ShareNutton.IsVisible = value;
                SetValue(IsShareVisibleProperty, value);
            }
        }

        protected virtual void OnShare()
        {
            Share?.Invoke(this, EventArgs.Empty);
        }

        private async void BackButton_OnTapped(object sender, EventArgs e)
        {
            var args = new NavigatingEventArgs(MenuItemsEnum.Back, false);
            OnNavigating(args);
            if(args.Cancel) return;
            await Navigation.MoveBack();
        }

        private void ShareButton_OnTapped(object sender, EventArgs e)
        {
            OnShare();
        }

        protected virtual void OnNavigating(NavigatingEventArgs e)
        {
            Navigating?.Invoke(this, e);
        }

        private void SendButton_OnTapped(object sender, EventArgs e)
        {
            OnSend();
        }

        protected virtual void OnSend()
        {
            Send?.Invoke(this, EventArgs.Empty);
        }
    }
}
