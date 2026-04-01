using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls
{
    /// <summary>
    /// Event args for MaterialDropDown selected item changes.
    /// Replaces Xamarin.Forms SelectedItemChangedEventArgs which does not exist in MAUI.
    /// </summary>
    public class MaterialDropDownSelectedEventArgs : EventArgs
    {
        public object SelectedItem { get; }
        public int SelectedIndex { get; }

        public MaterialDropDownSelectedEventArgs(object selectedItem, int selectedIndex)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
        }
    }

    public partial class MaterialDropDown : ContentView
    {
        public MaterialDropDown()
        {
            InitializeComponent();
        }

        public event EventHandler<MaterialDropDownSelectedEventArgs> SelectedItemChanged;
        public new event EventHandler<FocusEventArgs> Focused;
        public new event EventHandler<FocusEventArgs> Unfocused;

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(MaterialDropDown),
                null, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    ctrl.TextEntry.SelectedItem = newValue;
                });

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(MaterialDropDown),
                null, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    ctrl.TextEntry.ItemsSource = (IList)newValue;
                });

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static new BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MaterialDropDown),
                0.0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    ctrl.TextEntry.FontSize = (double)newValue;
                });

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static new BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MaterialDropDown),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    ctrl.TextEntry.FontFamily = (string)newValue;
                });

        public new string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public BindingBase ItemsDisplay
        {
            get => TextEntry.ItemDisplayBinding;
            set => TextEntry.ItemDisplayBinding = value;
        }

        public static BindableProperty ErrorStringProperty =
            BindableProperty.Create(nameof(ErrorString), typeof(string), typeof(MaterialDropDown),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    var val = (string)newValue;
                    ctrl.Error.Text = val;
                    ctrl.Error.Opacity = string.IsNullOrEmpty(val) ? 0 : 1;
                    ctrl.Underline.Color = string.IsNullOrEmpty(val)
                        ? ctrl.IsFocused ? ctrl.Color : CustomColors.GrayColor
                        : Colors.Red;
                });

        public string ErrorString
        {
            get => (string)GetValue(ErrorStringProperty);
            set => SetValue(ErrorStringProperty, value);
        }

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialDropDown),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    var val = (string)newValue;
                    ctrl.TextEntry.Title = val;
                    ctrl.PlaceholderLabel.Text = val;
                    ctrl.PlaceholderLabel.IsVisible = !string.IsNullOrEmpty(val);
                });

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(MaterialDropDown),
                Colors.Gray, BindingMode.TwoWay);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MaterialDropDown),
                Colors.White, BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(MaterialDropDown),
                CustomColors.GrayColor, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    var color = (Color)newValue;
                    ctrl.TextEntry.TextColor = color;
                    ctrl.Underline.Color = color;
                    ctrl.PlaceholderLabel.TextColor = color;
                });

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public static BindableProperty PrefixSourceProperty =
            BindableProperty.Create(nameof(PrefixSource), typeof(string), typeof(MaterialDropDown),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialDropDown)bindable;
                    ctrl.PrefixImage.IsVisible = true;
                    ctrl.PrefixImage.Source = ImageSource.FromFile((string)newValue);
                });

        public string PrefixSource
        {
            get => (string)GetValue(PrefixSourceProperty);
            set => SetValue(PrefixSourceProperty, value);
        }

        private void PlaceholderLabel_OnFocused(object sender, FocusEventArgs e)
        {
            TextEntry.TextColor = TextColor;
            Underline.Color = Color;
            PlaceholderLabel.TextColor = Color;
            OnFocused(e);
        }

        private void PlaceholderLabel_OnUnfocused(object sender, FocusEventArgs e)
        {
            TextEntry.TextColor = PlaceholderColor;
            Underline.Color = PlaceholderColor;
            PlaceholderLabel.TextColor = PlaceholderColor;
            OnUnfocused(e);
        }

        private void TextEntry_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TextEntry.SelectedItem == null) return;
            SetValue(SelectedItemProperty, TextEntry.SelectedItem);
            SelectedItemChanged?.Invoke(this, new MaterialDropDownSelectedEventArgs(TextEntry.SelectedItem, TextEntry.SelectedIndex));
        }

        protected virtual void OnFocused(FocusEventArgs e)
        {
            Focused?.Invoke(this, e);
        }

        protected virtual void OnUnfocused(FocusEventArgs e)
        {
            Unfocused?.Invoke(this, e);
        }
    }
}
