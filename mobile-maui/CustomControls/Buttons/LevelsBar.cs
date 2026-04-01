using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using CustomControls.Extensions;
using CustomControls.Extentions;
using CustomControls.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.Buttons
{
    public enum ButtonType
    {
        Right,
        Center,
        Left
    }

    public class LevelsBar : ContentView
    {
        public event EventHandler<LevelsBarEventArgs> Touched;

        private SKCanvasView levelsBar;
        private Dictionary<int,SKPath> levelsPaths = new Dictionary<int, SKPath>();

        private const double DipButtonWidth = 80;
        private const double DipButtonHeight = 30;
        private const long DipRoundRadius = 8;

        private static readonly double ButtonWidth = DeviceDisplay.MainDisplayInfo.Density * DipButtonWidth;
        private static readonly double ButtonHeight = DeviceDisplay.MainDisplayInfo.Density * DipButtonHeight;
        private static readonly long RoundRadius = (long)DeviceDisplay.MainDisplayInfo.Density * DipRoundRadius;
        private long borderWidth => BorderWidth*(long)DeviceDisplay.MainDisplayInfo.Density;

        private Rectangle realRect = new Rectangle(0,0,ButtonWidth*3, ButtonHeight);

        public LevelsBar()
        {
            levelsBar = new SKCanvasView();

            levelsBar.PaintSurface += ControlOnPaintSurface;
            levelsBar.EnableTouchEvents = true;
            levelsBar.Touch += ControlOnTouch;

            Content = levelsBar;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (Elements == null || Elements.Count > 3) return;
            levelsPaths = new Dictionary<int, SKPath>();
            realRect = e.Surface.Canvas.LocalClipBounds.ToFormsRect().LocateInCenter(realRect);
            var i = 0;
            foreach (var element in Elements)
            {
                levelsPaths.Add(element.Key, DrawLevelButton(element, i, new Rectangle(ButtonWidth * i + realRect.X, realRect.Y, ButtonWidth, ButtonHeight), e.Surface.Canvas));
                i++;
            }
        }

        private SKPath DrawLevelButton(ILevelBarElement element, int buttonCount, Rectangle rect, SKCanvas canvas)
        {
            ButtonType type;
            if (buttonCount == 0) type = ButtonType.Left;
            else if (buttonCount == (Elements.Count - 1)) type = ButtonType.Right;
            else type = ButtonType.Center;

            switch (type)
            {
                case ButtonType.Right:
                    return canvas.DrawRightRoundedButton(element.Name, FontSize,
                        element.IsSelected ? SelectedFontColor : FontColor, rect, borderWidth, BorderColor,
                        element.IsSelected ? SelectedFillColor : DefaultFillColor, RoundRadius);
                case ButtonType.Center:
                    return canvas.DrawSquareButton(element.Name, FontSize,
                        element.IsSelected ? SelectedFontColor : FontColor, rect, borderWidth, BorderColor,
                        element.IsSelected ? SelectedFillColor : DefaultFillColor);
                case ButtonType.Left:
                    return canvas.DrawLeftRoundedButton(element.Name, FontSize,
                        element.IsSelected ? SelectedFontColor : FontColor, rect, borderWidth, BorderColor,
                        element.IsSelected ? SelectedFillColor : DefaultFillColor, RoundRadius);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty DefaultFillColorProperty = BindableProperty.Create(nameof(DefaultFillColor),
            typeof(Color), typeof(LevelsBar), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.DefaultFillColor = (Color) newValue;
            });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public Color DefaultFillColor
        {
            get => (Color) GetValue(DefaultFillColorProperty);
            set => SetValue(DefaultFillColorProperty, value);
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty SelectedFillColorProperty = BindableProperty.Create(nameof(SelectedFillColor),
            typeof(Color), typeof(LevelsBar), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.SelectedFillColor = (Color) newValue;
            });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public Color SelectedFillColor
        {
            get => (Color) GetValue(SelectedFillColorProperty);
            set => SetValue(SelectedFillColorProperty, value);
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(LevelsBar), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
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

        public static BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(long),
            typeof(LevelsBar), (long)1, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.BorderWidth = (long) newValue;
            });

        public long BorderWidth
        {
            get => (long)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, (long)value);
        }

        public static BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int),
            typeof(LevelsBar), 0, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.FontSize = (int) newValue;
            });

        public int FontSize
        {
            get => (int) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty = BindableProperty.Create(nameof(FontColor),
            typeof(Color), typeof(LevelsBar), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.FontColor = (Color) newValue;
            });

        public Color FontColor
        {
            get => (Color) GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        public static BindableProperty SelectedFontColorProperty = BindableProperty.Create(nameof(SelectedFontColor),
            typeof(Color), typeof(LevelsBar), Colors.Gray, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.SelectedFontColor = (Color) newValue;
            });

        public Color SelectedFontColor
        {
            get => (Color)GetValue(SelectedFontColorProperty);
            set => SetValue(SelectedFontColorProperty, value);
        }


        public static BindableProperty ElementsProperty = BindableProperty.Create(nameof(Elements),
            typeof(List<ILevelBarElement>), typeof(LevelsBar), null, BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ctrl = (LevelsBar) bindable;
                ctrl.Elements = (List<ILevelBarElement>) newValue;
            });

        public List<ILevelBarElement> Elements
        {
            get => (List<ILevelBarElement>) GetValue(ElementsProperty);
            set
            {
                SetValue(ElementsProperty, value);
                levelsBar.InvalidateSurface();
            }
        }

        public void ForceInvalidate()
        {
            levelsBar.InvalidateSurface();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Width) || propertyName == nameof(Height))
                levelsBar.InvalidateSurface();
        }

        #region Touch event implementation

        public class LevelsBarEventArgs
        {
            public LevelsBarEventArgs(int key) { Key = key; }

            public int Key { get; set; }
        }

        protected virtual void RaiseTouchedEvent(int key)
        {
            Touched?.Invoke(this, new LevelsBarEventArgs(key));
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
                    //TouchEnd();
                    break;
                case SKTouchAction.Released:
                    var element = levelsPaths.FirstOrDefault(h => h.Value.Contains(e.Location.X, e.Location.Y));
                    if (element.Value == null) return;

                    RaiseTouchedEvent(element.Key);
                    //TouchEnd();
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

        #endregion //Touch event implementation
    }
}
