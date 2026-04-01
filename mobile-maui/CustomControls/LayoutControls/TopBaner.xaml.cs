using System;
using Microsoft.Maui.Controls;

namespace CustomControls.LayoutControls
{
    /// <summary>
    /// Top banner control variant used in PurchasesPage.
    /// Displays an emblem image, header text, and an optional about button.
    /// Different from BioBrain.Controls.TopBaner which is the standard header with back/share.
    /// </summary>
    public partial class TopBaner : ContentView
    {
        public event EventHandler EmblemTapped;

        public TopBaner()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TopBaner), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TopBaner)bindable;
                    ctrl.HeaderLabel.Text = (string)newValue;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(string), typeof(TopBaner), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TopBaner)bindable;
                    var source = (string)newValue;
                    ctrl.EmblemImage.Source = string.IsNullOrEmpty(source) ? null : ImageSource.FromFile(source);
                    ctrl.EmblemImage.IsVisible = !string.IsNullOrEmpty(source);
                });

        public string Image
        {
            get => (string)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static readonly BindableProperty IsAboutVisibleProperty =
            BindableProperty.Create(nameof(IsAboutVisible), typeof(bool), typeof(TopBaner), true, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TopBaner)bindable;
                    ctrl.AboutButton.IsVisible = (bool)newValue;
                });

        public bool IsAboutVisible
        {
            get => (bool)GetValue(IsAboutVisibleProperty);
            set => SetValue(IsAboutVisibleProperty, value);
        }

        private void EmblemImage_OnTapped(object sender, EventArgs e)
        {
            EmblemTapped?.Invoke(this, EventArgs.Empty);
        }

        private void AboutButton_OnTapped(object sender, EventArgs e)
        {
            // Navigate to about page if needed; currently a placeholder
        }
    }
}
