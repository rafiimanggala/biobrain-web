using Common;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.ImageControls
{
    public class SpeechBubbleControl : ContentView
    {
        private float RealBorderThickness => BorderThickness * (float)DeviceDisplay.MainDisplayInfo.Density;

        public SpeechBubbleControl()
        {
            HeightRequest = 60;
            WidthRequest = 60;


            var control = new SKCanvasView
            {
                HeightRequest = 60,
                WidthRequest = 60,
            };

            control.PaintSurface += ControlOnPaintSurface;

            Content = control;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var path = new SKPath();
            var initHeight = DeviceInfo.Platform == DevicePlatform.Android
                ? RealBorderThickness / 2
                : 0;
            path.MoveTo(0, initHeight + RealBorderThickness / 2);
            path.LineTo(e.Surface.Canvas.LocalClipBounds.Width - 4, e.Surface.Canvas.LocalClipBounds.Height);
            path.LineTo(e.Surface.Canvas.LocalClipBounds.Width * 0.85f, initHeight);
            //path.Close();

            e.Surface.Canvas.DrawPath(path, new SKPaint{Color = FillColor.ToSKColor(), Style = SKPaintStyle.Fill});
            e.Surface.Canvas.DrawPath(path, new SKPaint{Color = BorderColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = RealBorderThickness });
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(SpeechBubbleControl), Colors.White, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
        {
            var ctrl = (SpeechBubbleControl)bindable;
            ctrl.FillColor = (Color)newValue;
        });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the button.</value>
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set { SetValue(FillColorProperty, value);
                ((SKCanvasView)Content).InvalidateSurface();
            }
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SpeechBubbleControl), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
        {
            var ctrl = (SpeechBubbleControl)bindable;
            ctrl.BorderColor = (Color)newValue;
        });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public new Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set
            {
                SetValue(BorderColorProperty, value);
                ((SKCanvasView)Content).InvalidateSurface();
            }
        }

        public static BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(SpeechBubbleControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (SpeechBubbleControl)bindable;
                    ctrl.BorderThickness = (int)newValue;
                });

        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set
            {
                SetValue(BorderThicknessProperty, value);
                ((SKCanvasView)Content).InvalidateSurface();
            }
        }
    }
}
