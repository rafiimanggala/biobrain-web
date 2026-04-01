using System;
using CustomControls.Effects;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls
{
    public class TextFieldControl : ContentView
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public new event EventHandler<FocusEventArgs> Focused;
        public new event EventHandler<FocusEventArgs> Unfocused;
        public event EventHandler Tapped;

        private readonly SKCanvasView searchBackground;
        private Entry entry;
        private Label label;

        public TextFieldControl()
        {
            searchBackground = new SKCanvasView();
            searchBackground.PaintSurface += ControlOnPaintSurface;

            SetContent();
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.DrawRoundRect(e.Surface.Canvas.LocalClipBounds, 5, 5, new SKPaint { Color = FillColor.ToSKColor() });
        }

        private void SetContent()
        {
            entry = new Entry
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = FontColor,
                Margin = new Thickness(5, 0, 5, 0),
                Keyboard = Keyboard.Chat,
                PlaceholderColor = FillColor,
                IsEnabled = Editable
            };
            entry.TextChanged += EntryOnTextChanged;
            entry.Focused += EntryOnFocused;
            entry.Unfocused += EntryOnUnfocused;

            label = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = FontColor,
                Margin = new Thickness(5, 0, 5, 0),
                IsEnabled = Editable
            };
            label.Focused += EntryOnFocused;
            label.Unfocused += EntryOnUnfocused;
            var gesture = new TapGestureRecognizer();
            gesture.Tapped += GestureOnTapped;
            label.GestureRecognizers.Add(gesture);
            BorderEffect.SetHasNoBorder(entry, true);

            Content = new Grid
            {
                Children =
                {
                    searchBackground,
                    entry
                }
            };
        }

        private void GestureOnTapped(object sender, EventArgs eventArgs)
        {
            Tapped?.Invoke(this, EventArgs.Empty);
        }

        private void EntryOnUnfocused(object sender, FocusEventArgs focusEventArgs)
        {
            OnUnfocused(focusEventArgs);
        }

        private void EntryOnFocused(object sender, FocusEventArgs focusEventArgs)
        {
            OnFocused(focusEventArgs);
        }

        public static BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(TextFieldControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.FillColor = (Color)newValue;
                });

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(TextFieldControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set
            {
                SetValue(FontSizeProperty, value);
                entry.FontSize = value;
                label.FontSize = value;
            }
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(TextFieldControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set
            {
                SetValue(FontColorProperty, value);
                entry.TextColor = value;
                label.TextColor = value;
            }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TextFieldControl), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                entry.Text = value;
                label.Text = value;
            }
        }

        public static BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(TextFieldControl), string.Empty, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static BindableProperty EditableProperty =
            BindableProperty.Create(nameof(Editable), typeof(bool), typeof(TextFieldControl), true, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextFieldControl)bindable;
                    ctrl.Editable = (bool)newValue;
                });

        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set
            {
                SetValue(EditableProperty, value);
                if (value)
                {
                    Content = new Grid
                    {
                        Children =
                        {
                            searchBackground,
                            entry
                        }
                    };
                }
                else
                {
                    Content = new Grid
                    {
                        Children =
                        {
                            searchBackground,
                            label
                        }
                    };
                }
            }
        }

        private void EntryOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            entry.Text = textChangedEventArgs.NewTextValue;
            Text = textChangedEventArgs.NewTextValue;
            TextChanged?.Invoke(this, textChangedEventArgs);
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
