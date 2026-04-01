using System;
using System.Windows.Input;
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
	public class HexagonButton : ContentView
	{
		private readonly SKCanvasView hexagon;
	    private Rectangle realRect;
	    private float RealBorderThickness => BorderThickness * (float)DeviceDisplay.MainDisplayInfo.Density;

		public HexagonButton()
		{
            hexagon = new SKCanvasView();

            hexagon.PaintSurface += ControlOnPaintSurface;
            hexagon.EnableTouchEvents = true;
            hexagon.Touch += ControlOnTouch;

			Content = hexagon;
		}

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			realRect = DeviceInfo.Platform == DevicePlatform.Android ? e.Surface.Canvas.LocalClipBounds.ToFormsRect() : e.Surface.Canvas.LocalClipBounds.ToFormsRect().FixForBorder(RealBorderThickness);

            e.Surface.Canvas.DrawHexagon(realRect, FillColor, BorderColor, RealBorderThickness);
            e.Surface.Canvas.DrawText(Text, realRect, FontColor, FontSize);
		}

		public static BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HexagonButton), defaultBindingMode: BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (HexagonButton)bindable;
					ctrl.Command = (ICommand)newValue;
				});

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

		public static BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(HexagonButton), defaultBindingMode: BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) => {
					var ctrl = (HexagonButton)bindable;
					ctrl.CommandParameter = newValue;
				});

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

		public static BindableProperty FillColorProperty =
			BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(HexagonButton),
				Colors.Gray, BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (HexagonButton)bindable;
					ctrl.FillColor = (Color)newValue;
				});

		public Color FillColor
		{
			get => (Color)GetValue(FillColorProperty);
            set
			{
				SetValue(FillColorProperty, value);
				hexagon.InvalidateSurface();
			}
		}

		public static BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(HexagonButton),
                Colors.Transparent, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonButton)bindable;
                    ctrl.BorderColor = (Color)newValue;
                });

        public new Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set
            {
                SetValue(BorderColorProperty, value);
                hexagon.InvalidateSurface();
            }
        }

        public static BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(HexagonButton), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonButton)bindable;
                    ctrl.BorderThickness = (int)newValue;
                });

        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set
            {
                SetValue(BorderThicknessProperty, value);
                hexagon.InvalidateSurface();
            }
        }

        public static BindableProperty FontSizeProperty =
			BindableProperty.Create(nameof(FontSize), typeof(int), typeof(HexagonButton), 0, BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (HexagonButton)bindable;
					ctrl.FontSize = (int)newValue;
				});

		public int FontSize
		{
			get => (int)GetValue(FontSizeProperty);
            set
			{
				SetValue(FontSizeProperty, value);
				hexagon.InvalidateSurface();
			}
		}

		public static BindableProperty FontColorProperty =
			BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(HexagonButton),
				Colors.Gray, BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (HexagonButton)bindable;
					ctrl.FontColor = (Color)newValue;
				});

		public Color FontColor
		{
			get => (Color)GetValue(FontColorProperty);
            set
			{
				SetValue(FontColorProperty, value);
				hexagon.InvalidateSurface();
			}
		}

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(HexagonButton), string.Empty, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonButton)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                hexagon.InvalidateSurface();
            }
        }

        public static BindableProperty IsAnimatedProperty =
            BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(HexagonButton), true, BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonButton)bindable;
                    ctrl.IsAnimated = (bool)newValue;
                });

        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public bool IsDrawTick { get; set; }

        #region Touch event implementation
        protected virtual void RaiseTouchedEvent()
        {
            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
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
            if (!IsAnimated) return;
            this.ScaleTo(0.8, 65, Easing.CubicInOut);
        }


        private void TouchEnd()
        {
            if (!IsAnimated) return;
            this.ScaleTo(1.0, 65, Easing.CubicInOut);
        }
        #endregion //Touch event implementation
	}
}
