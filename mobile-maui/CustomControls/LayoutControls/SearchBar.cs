using System;
using CustomControls.Effects;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.LayoutControls
{
    public class SearchBar : ContentView
    {
        public event EventHandler<SearchEventArgs> OnSearch;

        private readonly SKCanvasView searchBackground;
        private Entry entry;

        public SearchBar()
        {
            searchBackground = new SKCanvasView();
            searchBackground.PaintSurface += ControlOnPaintSurface;

            SetContent();
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var radius = e.Surface.Canvas.LocalClipBounds.Height / 2;
            e.Surface.Canvas.DrawRoundRect(
                e.Surface.Canvas.LocalClipBounds,
                new SKSize(radius, radius),
                new SKPaint { Color = FillColor.ToSKColor() });
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsVisible))
            {
                searchBackground.InvalidateSurface();
            }
        }

        private void SetContent()
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += SearchButton_OnTapped;

            entry = new Entry
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = FontColor,
                Margin = new Thickness(0, 0, 20, 0),
                Keyboard = Keyboard.Chat,
                PlaceholderColor = FillColor
            };
            entry.Completed += EntryOnCompleted;
            entry.TextChanged += EntryOnTextChanged;
            BorderEffect.SetHasNoBorder(entry, true);

            Content = new Grid
            {
                Children =
                {
                    searchBackground,
                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Image
                            {
                                HorizontalOptions = LayoutOptions.Start,
                                HeightRequest = 30,
                                Margin = new Thickness(10, 5, 5, 5),
                                Source = ImageSource.FromFile("SearchIcon.png"),
                                GestureRecognizers = { tapGesture }
                            },
                            entry
                        }
                    }
                }
            };
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(SearchBar),
                Colors.White, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SearchBar)bindable;
                    ctrl.FillColor = (Color)newValue;
                });

        /// <summary>
        /// Gets or sets the FillColor of the SearchBar instance.
        /// </summary>
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(SearchBar), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SearchBar)bindable;
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
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(SearchBar),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SearchBar)bindable;
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
            BindableProperty.Create(nameof(Text), typeof(string), typeof(SearchBar), string.Empty, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SearchBar)bindable;
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
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SearchBar), string.Empty, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SearchBar)bindable;
                    ctrl.FontFamily = (string)newValue;
                });

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        private void EntryOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            entry.Text = textChangedEventArgs.NewTextValue.ToUpper();
        }

        private void EntryOnCompleted(object sender, EventArgs eventArgs)
        {
            InvokeOnSearch(entry.Text);
        }

        private void SearchButton_OnTapped(object sender, EventArgs eventArgs)
        {
            InvokeOnSearch(entry.Text);
        }

        private void InvokeOnSearch(string searchText)
        {
            OnSearch?.Invoke(this, new SearchEventArgs { SearchText = searchText });
        }
    }
}
