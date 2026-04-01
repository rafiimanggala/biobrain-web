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

namespace CustomControls.LayoutControls
{
    public class HexagonResultControl : ContentView
    {
        public event EventHandler<HexagonResultControlEventArgs> Touched;

        private const double BaseWidth = 295;
        private const double BaseHeight = 210;
        private const double MaxCoefficient = 0.9;

        private double koef = 0.6;

        private double DipHexWidth => 70.0 * koef;
        private double DipHexHeight => 80.0 * koef;
        private double DipD => 5.0 * koef;

        //Distance between hexagons
        private double D => Settings.Density * DipD;
        private double HexWidth => Settings.Density * DipHexWidth;
        private double HexHeight => Settings.Density * DipHexHeight;

        private Rectangle realRect;
        private Dictionary<int, SKPath> hexagons;

        private int selectedQuestion = -1;

        private readonly SKCanvasView control;

        public HexagonResultControl()
        {
            WidthRequest = DipHexWidth * 4 + DipD * 3;
            HeightRequest = DipHexHeight * 2.5 + DipD * 2;

            control = new SKCanvasView();

            control.PaintSurface += ControlOnPaintSurface;
            control.EnableTouchEvents = true;
            control.Touch += ControlOnTouch;

            Content = control;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();
            var hCoeff = Height / BaseHeight;
            var wCoeff = Width / BaseWidth;
            var coeff = Math.Min(wCoeff, hCoeff);
            koef = Math.Min(Math.Max(coeff, koef), MaxCoefficient);
            CalculateRealRect(e.Surface.Canvas.LocalClipBounds.ToFormsRect());
            hexagons = new Dictionary<int, SKPath>();
            for (var i = 0; i < Elements.Count && i < 10; i++)
            {
                double dx;
                double dy;

                if (i > 2 && i < 7)
                {
                    dx = (i - 3) * (HexWidth + D);
                    dy = 0.75 * HexHeight + D;
                }
                else
                {
                    if (i < 3)
                    {
                        dx = (0.5 + i) * (HexWidth + D);
                        dy = 0;
                    }
                    else
                    {
                        dx = (0.5 + i - 7) * (HexWidth + D);
                        dy = 1.5 * HexHeight + 2 * D;
                    }
                }

                var hexagonRect = new Rectangle(dx + realRect.X, dy + realRect.Y, HexWidth, HexHeight);
                hexagons.Add(Elements[i].QuestionID,
                    e.Surface.Canvas.DrawHexagon(hexagonRect, Elements[i].Color, Elements[i].Color, 0));
                e.Surface.Canvas.DrawText(Elements[i].Name, hexagonRect, FontColor, FontSize);
            }
        }

        protected override void InvalidateLayout()
        {
            base.InvalidateLayout();
            control.InvalidateSurface();
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            control.InvalidateSurface();
        }

        private void CalculateRealRect(Rectangle layoutRect)
        {
            realRect = new Rectangle(0, 0, HexWidth * 4 + D * 3, HexHeight * 2.5 + D * 2);
            var x = (layoutRect.Width - realRect.Width) / 2;
            var y = (layoutRect.Height - realRect.Height) / 2;
            realRect = new Rectangle(new Point(x, y), realRect.Size);
        }

        public List<IResultElement> Elements
        {
            get => (List<IResultElement>)GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }

        public static BindableProperty ElementsProperty =
            BindableProperty.Create(nameof(Elements), typeof(List<IResultElement>), typeof(HexagonResultControl),
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonResultControl)bindable;
                    ctrl.Elements = (List<IResultElement>)newValue;
                });

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(HexagonResultControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonResultControl)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(HexagonResultControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (HexagonResultControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        #region Touch event implementation

        protected virtual void RaiseTouchedEvent(int id)
        {
            Touched?.Invoke(this, new HexagonResultControlEventArgs(id));
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
                    if (selectedQuestion != -1)
                        RaiseTouchedEvent(selectedQuestion);
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
            var element = hexagons.FirstOrDefault(h => h.Value.Contains(point.X, point.Y));

            if (element.Value == null)
                TouchEnd();
            else
            {
                selectedQuestion = element.Key;
                control.InvalidateSurface();
            }
        }

        private void TouchEnd()
        {
            selectedQuestion = -1;
            control.InvalidateSurface();
        }

        #endregion //Touch event implementation
    }
}
