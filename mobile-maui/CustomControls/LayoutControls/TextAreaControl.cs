using System;
using CustomControls.Effects;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls
{
    public class TextAreaControl : ContentView
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public new event EventHandler<FocusEventArgs> Focused;
        public new event EventHandler<FocusEventArgs> Unfocused;
        public event EventHandler Tapped;

        private readonly SKCanvasView searchBackground;
        private Editor entry;

        public TextAreaControl()
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
            entry = new Editor
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = FontColor,
                Margin = new Thickness(5, 0, 5, 0),
                Keyboard = Keyboard.Chat,
            };
            entry.TextChanged += EntryOnTextChanged;
            entry.Focused += EntryOnFocused;
            entry.Unfocused += EntryOnUnfocused;

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

        private void EntryOnUnfocused(object sender, FocusEventArgs focusEventArgs)
        {
            OnUnfocused(focusEventArgs);
        }

        private void EntryOnFocused(object sender, FocusEventArgs focusEventArgs)
        {
            OnFocused(focusEventArgs);
        }

        public static BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(TextAreaControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextAreaControl)bindable;
                    ctrl.FillColor = (Color)newValue;
                });

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(TextAreaControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextAreaControl)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set
            {
                SetValue(FontSizeProperty, value);
                entry.FontSize = value;
            }
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(TextAreaControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextAreaControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set
            {
                SetValue(FontColorProperty, value);
                entry.TextColor = value;
            }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TextAreaControl), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextAreaControl)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                entry.Text = value;
            }
        }

        public static BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(TextAreaControl), string.Empty, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (TextAreaControl)bindable;
                    ctrl.FontFamily = (string)newValue;
                });

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
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

        protected virtual void OnTapped()
        {
            Tapped?.Invoke(this, EventArgs.Empty);
        }
    }
}
