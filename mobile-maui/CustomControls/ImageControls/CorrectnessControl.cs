using System;
using System.Reflection;
using Common;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.ImageControls
{
    // Note: Original Xamarin file used a Cyrillic 'С' (U+0421) in the class name.
    // MAUI version uses standard ASCII 'C' for CorrectnessControl.
    public class CorrectnessControl : ContentView
    {
        private readonly SKCanvasView correctness;
        private readonly double borderRadius = 2 * Settings.Density;
        private Rectangle realRect;
        public event EventHandler OnTouched;

        public CorrectnessControl()
        {
            HeightRequest = 50;
            WidthRequest = 50;
            correctness = new SKCanvasView
            {
                HeightRequest = 50,
                WidthRequest = 50,
            };

            correctness.PaintSurface += ControlOnPaintSurface;
            correctness.EnableTouchEvents = true;
            correctness.Touch += ControlOnTouch;

            Content = correctness;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SetRealRect(e.Surface.Canvas.LocalClipBounds.ToFormsRect());

            e.Surface.Canvas.DrawOval(realRect.ToSKRect(), new SKPaint { Color = (IsCorrect ? CorrectFillColor : WrongFillColor).ToSKColor(), Style = SKPaintStyle.Fill });
            e.Surface.Canvas.DrawOval(realRect.ToSKRect(), new SKPaint { Color = Colors.White.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = (float)borderRadius });
            var btm = LoadBitmap();
            if (btm != null)
            {
                var img = SKImage.FromBitmap(btm);
                e.Surface.Canvas.DrawImage(img, GetImageRect(realRect).ToSKRect());
            }
        }

        private SKBitmap LoadBitmap()
        {
            string resourceID = IsCorrect ? "TickImage.png" : "CrossImage.png";
            var assembly = GetType().GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceID))
            {
                if (stream == null) return null;
                return SKBitmap.Decode(stream);
            }
        }

        private void SetRealRect(Rectangle rect)
        {
            realRect = new Rectangle(
                rect.X + borderRadius,
                rect.Y + borderRadius,
                rect.Width - 2 * borderRadius,
                rect.Height - 2 * borderRadius);
        }

        private Rectangle GetImageRect(Rectangle controlRect)
        {
            return new Rectangle(controlRect.X + controlRect.Width / 4, controlRect.Y + controlRect.Height / 4, controlRect.Width / 2, controlRect.Height / 2);
        }

        public static BindableProperty CorrectFillColorProperty =
            BindableProperty.Create(nameof(CorrectFillColor), typeof(Color), typeof(CorrectnessControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (CorrectnessControl)bindable;
                    ctrl.CorrectFillColor = (Color)newValue;
                });

        public Color CorrectFillColor
        {
            get => (Color)GetValue(CorrectFillColorProperty);
            set => SetValue(CorrectFillColorProperty, value);
        }

        public static BindableProperty WrongFillColorProperty =
            BindableProperty.Create(nameof(WrongFillColor), typeof(Color), typeof(CorrectnessControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (CorrectnessControl)bindable;
                    ctrl.WrongFillColor = (Color)newValue;
                });

        public Color WrongFillColor
        {
            get => (Color)GetValue(WrongFillColorProperty);
            set
            {
                SetValue(WrongFillColorProperty, value);
                correctness.InvalidateSurface();
            }
        }

        public static BindableProperty IsCorrectProperty =
            BindableProperty.Create(nameof(IsCorrect), typeof(bool), typeof(CorrectnessControl), false, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (CorrectnessControl)bindable;
                    ctrl.IsCorrect = (bool)newValue;
                });

        public bool IsCorrect
        {
            get => (bool)GetValue(IsCorrectProperty);
            set
            {
                SetValue(IsCorrectProperty, value);
                correctness.InvalidateSurface();
            }
        }

        private void ControlOnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Entered:
                    break;
                case SKTouchAction.Pressed:
                    TouchesBegan(e.Location);
                    break;
                case SKTouchAction.Moved:
                    TouchesBegan(e.Location);
                    break;
                case SKTouchAction.Released:
                    OnTouched?.Invoke(this, new TappedEventArgs(null));
                    break;
                case SKTouchAction.Cancelled:
                    TouchEnd();
                    break;
                case SKTouchAction.Exited:
                    TouchEnd();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // we have handled these events
            e.Handled = true;
        }

        private void TouchesBegan(SKPoint point)
        {
        }

        private void TouchEnd()
        {
        }
    }
}
