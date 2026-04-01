using Common;
using CustomControls.Extensions;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.LayoutControls
{
    public class QuestionPager : ContentView
    {
        private const double DipCircleSize = 6;
        private const double DipVerticalSpace = 6;
        private const double DipHorizontalSpace = 3;

        private readonly double circleSize = Settings.Density * DipCircleSize;
        private readonly double verticalSpace = Settings.Density * DipVerticalSpace;
        private readonly double horizontalSpace = Settings.Density * DipHorizontalSpace;

        private Rectangle realRect;

        private SKCanvasView pager;

        public QuestionPager()
        {
            HeightRequest = 80;
            WidthRequest = 70;
            MinimumWidthRequest = 80;

            pager = new SKCanvasView();

            pager.PaintSurface += ControlOnPaintSurface;
            pager.EnableTouchEvents = false;

            Content = pager;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            CalcRealRect(e.Surface.Canvas.DeviceClipBounds.ToFormsRect());
            var circlesRect = GetCirclesRect(e.Surface.Canvas.DeviceClipBounds.ToFormsRect());
            var circleYCoord = circlesRect.Y + circlesRect.Height / 2;
            for (var i = 0; i < NumberOfPages; i++)
            {
                var circleRect = new Rectangle(i * (circleSize + horizontalSpace) + 1 * Settings.Density + realRect.X, circleYCoord, circleSize, circleSize);
                e.Surface.Canvas.DrawOval(circleRect.ToSKRect(),
                    i + 1 == CurrentPage
                        ? new SKPaint { Color = SelectedCircleColor.ToSKColor(), Style = SKPaintStyle.Fill }
                        : new SKPaint { Color = DefaultCircleColor.ToSKColor(), Style = SKPaintStyle.Fill });
            }
            var textRect = GetTextRect(realRect);
            e.Surface.Canvas.DrawText(Text, textRect, FontColor, FontSize);
        }

        private void CalcRealRect(Rectangle rect)
        {
            var width = circleSize * NumberOfPages + horizontalSpace * (NumberOfPages - 1);
            var height = circleSize + verticalSpace + FontSize * Settings.Density;
            var x = (rect.Width - width) / 2;
            var y = (rect.Height - height) / 2;
            realRect = new Rectangle(x > 0 ? x : 0, y > 0 ? y : 0, width, height);
        }

        private Rectangle GetCirclesRect(Rectangle rect)
        {
            var width = rect.Width;
            var height = rect.Height / 2 - verticalSpace / 2;
            return new Rectangle(rect.X, 0, width, height);
        }

        private Rectangle GetTextRect(Rectangle rect)
        {
            var width = rect.Width;
            var height = rect.Height / 2 - verticalSpace / 2;
            return new Rectangle(rect.X, rect.Height / 2 + verticalSpace / 2, width, height);
        }

        public static BindableProperty DefaultCircleColorProperty =
            BindableProperty.Create(nameof(DefaultCircleColor), typeof(Color), typeof(QuestionPager),
                Colors.White, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.DefaultCircleColor = (Color)newValue;
                });

        public Color DefaultCircleColor
        {
            get => (Color)GetValue(DefaultCircleColorProperty);
            set => SetValue(DefaultCircleColorProperty, value);
        }

        public static BindableProperty SelectedCircleColorProperty =
            BindableProperty.Create(nameof(SelectedCircleColor), typeof(Color), typeof(QuestionPager),
                Color.FromArgb("#38A6E3"), BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.SelectedCircleColor = (Color)newValue;
                });

        public Color SelectedCircleColor
        {
            get => (Color)GetValue(SelectedCircleColorProperty);
            set => SetValue(SelectedCircleColorProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(QuestionPager), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(QuestionPager),
                Colors.White, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(QuestionPager), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                pager.InvalidateSurface();
            }
        }

        public static BindableProperty NumberOfPagesProperty =
            BindableProperty.Create(nameof(NumberOfPages), typeof(int), typeof(QuestionPager), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.NumberOfPages = (int)newValue;
                });

        public int NumberOfPages
        {
            get => (int)GetValue(NumberOfPagesProperty);
            set
            {
                SetValue(NumberOfPagesProperty, value);
                pager.InvalidateSurface();
            }
        }

        public static BindableProperty CurrentPageProperty =
            BindableProperty.Create(nameof(CurrentPage), typeof(int), typeof(QuestionPager), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (QuestionPager)bindable;
                    ctrl.CurrentPage = (int)newValue;
                });

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set
            {
                SetValue(CurrentPageProperty, value);
                pager.InvalidateSurface();
            }
        }
    }
}
