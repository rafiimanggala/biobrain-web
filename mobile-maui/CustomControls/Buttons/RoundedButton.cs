using System;
using Common;
using CustomControls.Extensions;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.Buttons
{
    public enum RoundedButtonType
    {
        LeftRounded,
        RightRounded,
        Both
    }

    public class RoundedButton : ContentView
    {
        private readonly SKCanvasView button;
        private const float DipRoundRadius = 12;
        private static readonly float RoundRadius = (float)DeviceDisplay.MainDisplayInfo.Density * DipRoundRadius;
        private static readonly float BorderRadius = (float)DeviceDisplay.MainDisplayInfo.Density * 4;
        private float DiagramPadding => BorderRadius / 2 + 1 * (float)DeviceDisplay.MainDisplayInfo.Density;

        private Rectangle realRect;

        public event EventHandler<object> Touched;

        public RoundedButton()
        {
            this.WidthRequest = 500;
            this.HeightRequest = 35;

            button = new SKCanvasView();

            button.PaintSurface += ControlOnPaintSurface;
            button.EnableTouchEvents = true;
            button.Touch += ControlOnTouch;

            Content = button;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SetRealRect(e.Surface.Canvas.LocalClipBounds.ToFormsRect());

            switch (Type)
            {
                case RoundedButtonType.LeftRounded:
                    e.Surface.Canvas.DrawLeftRoundedButton(Text, FontSize, FontColor, realRect, (long)BorderRadius, BorderColor, FillColor, (long)RoundRadius);
                    break;
                case RoundedButtonType.RightRounded:
                    e.Surface.Canvas.DrawRightRoundedButton(Text, FontSize, FontColor, realRect, (long)BorderRadius, BorderColor, FillColor, (long)RoundRadius);
                    break;
                case RoundedButtonType.Both:
                    e.Surface.Canvas.DrawBothRoundedButton(Text, FontSize, FontColor, realRect, (long)BorderRadius, BorderColor, FillColor, (long)RoundRadius);
                    break;
            }
        }

        private void SetRealRect(Rectangle rect)
        {
            realRect = new Rectangle();
            switch (Type)
            {
                case RoundedButtonType.LeftRounded:
                    realRect = new Rectangle(rect.X + BorderRadius, rect.Y + BorderRadius, rect.Width - 2 * BorderRadius, rect.Height - 2*BorderRadius);
                    break;
                case RoundedButtonType.RightRounded:
                    realRect = new Rectangle(rect.X + BorderRadius, rect.Y + BorderRadius, rect.Width - 2 * BorderRadius, rect.Height - 2*BorderRadius);
                    break;
                case RoundedButtonType.Both:
                    realRect = new Rectangle(rect.X + BorderRadius, rect.Y + BorderRadius, rect.Width - 2 * BorderRadius, rect.Height - 2 * BorderRadius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsEnabled))
                Opacity = IsEnabled ? 1 : 0.3;
        }

        /// <summary>
        /// The CommandParameter property.
        /// </summary>
        public static BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof (object), typeof (RoundedButton), defaultBindingMode: BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var ctrl = (RoundedButton) bindable;
            ctrl.CommandParameter = newValue;
        });

        /// <summary>
        /// Gets or sets the CommandParameter of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color),
            typeof(RoundedButton), Colors.Gray, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.FillColor = (Color) newValue;
            });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public Color FillColor
        {
            get => (Color) GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color),
            typeof(RoundedButton), Colors.Gray, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.BorderColor = (Color) newValue;
            });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public new Color BorderColor
        {
            get => (Color) GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(RoundedButtonType),
            typeof(RoundedButton), RoundedButtonType.LeftRounded, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.Type = (RoundedButtonType) newValue;
            });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public RoundedButtonType Type
        {
            get => (RoundedButtonType) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public static BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int),
            typeof(RoundedButton), 0, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.FontSize = (int) newValue;
            });

        public int FontSize
        {
            get => (int) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty = BindableProperty.Create(nameof(FontColor), typeof(Color),
            typeof(RoundedButton), Colors.Gray, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.FontColor = (Color) newValue;
            });

        public Color FontColor
        {
            get => (Color) GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(RoundedButton), string.Empty, BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (RoundedButton) bindable;
                ctrl.Text = (string) newValue;
            });

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                //To avoid app crash when working in background (sett button text when picking photo, but activity destroyd by Android)
                try
                {
                    button.InvalidateSurface();
                }
                catch
                {
                }
            }
        }

        private void ControlOnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Entered:
	                TouchEnd();
                    break;
                case SKTouchAction.Pressed:
                    TouchesBegan(e.Location);
                    break;
                case SKTouchAction.Moved:
                    TouchesBegan(e.Location);
                    TouchEnd();
                    break;
                case SKTouchAction.Released:
                    RaiseTouchedEvent();
                    TouchEnd();
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
            this.ScaleTo(0.9, 65, Easing.CubicInOut);
        }


        private void TouchEnd()
        {
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
        }

        private void RaiseTouchedEvent()
        {
            if (!IsEnabled) return;
            Touched?.Invoke(this, CommandParameter);
        }
    }
}
