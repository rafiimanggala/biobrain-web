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
    public class PersentDiagramControl : ContentView
    {
        private Rectangle realRect;
        private Rectangle textRect;

        private float DiagramStrokeWidthDip => 8;
        private float DiagramStrokeWidth => DiagramStrokeWidthDip * Settings.Density;

        public PersentDiagramControl()
        {
            WidthRequest = 120;
            HeightRequest = 120;

            var control = new SKCanvasView();

            control.PaintSurface += ControlOnPaintSurface;
            control.EnableTouchEvents = false;

            Content = control;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            realRect = new Rectangle(DiagramStrokeWidth, DiagramStrokeWidth, WidthRequest * Settings.Density - 2 * DiagramStrokeWidth, HeightRequest * Settings.Density - 2 * DiagramStrokeWidth);
            textRect = new Rectangle(0, 0, WidthRequest * Settings.Density - DiagramStrokeWidth, HeightRequest * Settings.Density - DiagramStrokeWidth);

            SetRealRect(e.Surface.Canvas.LocalClipBounds.ToFormsRect());

            using (SKPath path = new SKPath())
            {
                path.AddArc(new SKRect((float)realRect.X, (float)realRect.Y, (float)(realRect.X + realRect.Width), (float)(realRect.Y + realRect.Height)), 270, 360);
                e.Surface.Canvas.DrawPath(path, new SKPaint { Color = Percent == 0 ? AccentColor.ToSKColor() : MainColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = DiagramStrokeWidth, StrokeCap = SKStrokeCap.Round });
            }
            using (SKPath path = new SKPath())
            {
                path.AddArc(new SKRect((float)realRect.X, (float)realRect.Y, (float)(realRect.X + realRect.Width), (float)(realRect.Y + realRect.Height)), 270, (float)(3.6 * Percent));
                e.Surface.Canvas.DrawPath(path, new SKPaint { Color = AccentColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = DiagramStrokeWidth, StrokeCap = SKStrokeCap.Square });
            }
            e.Surface.Canvas.DrawText(Percent + "%", realRect, FontColor, FontSize);
        }

        private void SetRealRect(Rectangle rect)
        {
            var x = (rect.Width - realRect.Width) / 2;
            var y = (rect.Height - realRect.Height) / 2;
            realRect = new Rectangle(x < 0 ? 0 : x, y < 0 ? 0 : y, realRect.Width, realRect.Height);
        }

        public static BindableProperty PercentProperty =
            BindableProperty.Create(nameof(Percent), typeof(double), typeof(PersentDiagramControl), 0.0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (PersentDiagramControl)bindable;
                    ctrl.Percent = (double)newValue;
                });

        public double Percent
        {
            get => (double)GetValue(PercentProperty);
            set
            {
                SetValue(PercentProperty, value);
                ((SKCanvasView)Content).InvalidateSurface();
            }
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(PersentDiagramControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (PersentDiagramControl)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(PersentDiagramControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (PersentDiagramControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        public Color MainColor
        {
            get => (Color)GetValue(MainColorProperty);
            set => SetValue(MainColorProperty, value);
        }

        public static BindableProperty MainColorProperty =
            BindableProperty.Create(nameof(MainColor), typeof(Color), typeof(PersentDiagramControl),
                CustomColors.Red, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (PersentDiagramControl)bindable;
                    ctrl.MainColor = (Color)newValue;
                });

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public static BindableProperty AccentColorProperty =
            BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(PersentDiagramControl),
                CustomColors.DarkGreen, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (PersentDiagramControl)bindable;
                    ctrl.AccentColor = (Color)newValue;
                });
    }
}
