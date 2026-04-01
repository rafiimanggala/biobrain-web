using System;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls
{
    public partial class MaterialInput : ContentView
    {
        public MaterialInput()
        {
            InitializeComponent();
            IsDigitsOnly = false;
            IsFormatedRealPart = false;
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;
        public new event EventHandler<FocusEventArgs> Focused;
        public new event EventHandler<FocusEventArgs> Unfocused;

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(MaterialInput),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.Text = (string)newValue;
                    ctrl.PlaceholderLabel.Opacity = string.IsNullOrEmpty((string)newValue) ? 0 : 1;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static BindableProperty ErrorStringProperty =
            BindableProperty.Create(nameof(ErrorString), typeof(string), typeof(MaterialInput),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    var val = (string)newValue;
                    ctrl.Error.Text = val;
                    ctrl.Error.Opacity = string.IsNullOrEmpty(val) ? 0 : 1;
                    ctrl.Underline.Color = string.IsNullOrEmpty(val)
                        ? ctrl.IsFocused ? ctrl.Color : ctrl.PlaceholderColor
                        : Colors.Red;
                });

        public string ErrorString
        {
            get => (string)GetValue(ErrorStringProperty);
            set => SetValue(ErrorStringProperty, value);
        }

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(MaterialInput),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    var val = (string)newValue;
                    ctrl.TextEntry.Placeholder = val;
                    ctrl.PlaceholderLabel.Text = val;
                    if (ctrl.IsFloatLabel)
                        ctrl.PlaceholderLabel.IsVisible = !string.IsNullOrEmpty(val);
                });

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(MaterialInput),
                CustomColors.GrayColor, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    var color = (Color)newValue;
                    ctrl.TextEntry.PlaceholderColor = color;
                    ctrl.TextEntry.TextColor = color;
                    ctrl.Underline.Color = color;
                    ctrl.PlaceholderLabel.TextColor = color;
                });

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public static BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(MaterialInput),
                Colors.Gray, BindingMode.TwoWay);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MaterialInput),
                Colors.White, BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static BindableProperty HorizontalTextAlignmentProperty =
            BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(MaterialInput),
                TextAlignment.Start, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.HorizontalTextAlignment = (TextAlignment)newValue;
                });

        public TextAlignment HorizontalTextAlignment
        {
            get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }

        public static new BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MaterialInput),
                0.0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.FontSize = (double)newValue;
                });

        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static new BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MaterialInput),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.FontFamily = (string)newValue;
                });

        public new string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static BindableProperty IsPasswordProperty =
            BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(MaterialInput),
                false, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.IsPassword = (bool)newValue;
                    ctrl.HideImage.IsVisible = (bool)newValue;
                });

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public static BindableProperty IsFloatLabelProperty =
            BindableProperty.Create(nameof(IsFloatLabel), typeof(bool), typeof(MaterialInput),
                false, BindingMode.TwoWay);

        public bool IsFloatLabel
        {
            get => (bool)GetValue(IsFloatLabelProperty);
            set => SetValue(IsFloatLabelProperty, value);
        }

        public static BindableProperty KeyboardProperty =
            BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(MaterialInput),
                Keyboard.Default, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.TextEntry.Keyboard = (Keyboard)newValue;
                });

        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public static BindableProperty PrefixSourceProperty =
            BindableProperty.Create(nameof(PrefixSource), typeof(string), typeof(MaterialInput),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (MaterialInput)bindable;
                    ctrl.PrefixImage.IsVisible = true;
                    ctrl.PrefixImage.Source = ImageSource.FromFile((string)newValue);
                });

        public string PrefixSource
        {
            get => (string)GetValue(PrefixSourceProperty);
            set => SetValue(PrefixSourceProperty, value);
        }

        public bool IsDigitsOnly { get; set; }
        public int NumberAfterDot { get; set; }
        public bool IsFormatedRealPart { get; set; }
        public bool IsEmpty => string.IsNullOrEmpty(TextEntry.Text);

        private void TextEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsDigitsOnly && !string.IsNullOrEmpty(e.NewTextValue))
            {
                var regex = new Regex(@"^(?<int>[0-9]*)(?:[\.,](?<real>[0-9]*))?$");
                var match = regex.Match(e.NewTextValue);
                if (!match.Success)
                {
                    TextEntry.Text = e.OldTextValue;
                    return;
                }

                if (e.NewTextValue[e.NewTextValue.Length - 1] == '.' || e.NewTextValue[e.NewTextValue.Length - 1] == ',' ||
                    e.NewTextValue[0] == '.' || e.NewTextValue[0] == ',')
                    return;

                if (IsFormatedRealPart && match.Groups["real"].Success && match.Groups["real"].Captures[0].Length > NumberAfterDot)
                {
                    var index = match.Groups["real"].Captures[0].Index;
                    TextEntry.Text = TextEntry.Text.Remove(index) + match.Groups["real"].Captures[0].Value.Substring(0, NumberAfterDot);
                    return;
                }
            }

            OnPropertyChanged(propertyName: nameof(IsEmpty));
            SetValue(TextProperty, e.NewTextValue);
            OnTextChanged(e);
            if (IsFloatLabel)
                PlaceholderLabel.Opacity = string.IsNullOrEmpty(e.NewTextValue) ? 0 : 1;
        }

        protected virtual void OnTextChanged(TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
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
            if (!string.IsNullOrEmpty(TextEntry.Text))
            {
                if (IsDigitsOnly || IsFormatedRealPart)
                {
                    if (TextEntry.Text.StartsWith(".") || TextEntry.Text.StartsWith(","))
                        TextEntry.Text = $"0{TextEntry.Text}";
                    if (TextEntry.Text.EndsWith(".") || TextEntry.Text.EndsWith(","))
                        TextEntry.Text = TextEntry.Text.Substring(0, TextEntry.Text.Length - 1);
                    if (string.IsNullOrEmpty(TextEntry.Text)) TextEntry.Text = "0";
                }
            }
            OnUnfocused(e);
        }

        protected virtual void OnFocused(FocusEventArgs e)
        {
            Focused?.Invoke(this, e);
        }

        protected virtual void OnUnfocused(FocusEventArgs e)
        {
            Unfocused?.Invoke(this, e);
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            TextEntry.IsPassword = !TextEntry.IsPassword;
        }
    }
}
