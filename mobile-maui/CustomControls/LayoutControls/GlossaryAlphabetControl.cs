using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using CustomControls.Extensions;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rect;
using Size = Microsoft.Maui.Graphics.Size;
using Point = Microsoft.Maui.Graphics.Point;

namespace CustomControls.LayoutControls
{
    public class GlossaryAlphabetControl : ContentView
    {
        public event EventHandler<AlphabetControlEventArgs> Touched;

        #region Private variables

        private const double BaseWidth = 445;
        private const double BaseHeight = 600;
        private const double MaxCoefficient = 1.2;

        private double sizeCoefficient = 0.55;
        private double DipHexWidth => 70.0 * sizeCoefficient;
        private double DipHexHeight => 80.0 * sizeCoefficient;
        private double DipD => 5.0 * sizeCoefficient;

        //Distance between hexagons
        private double D => Settings.Density * DipD;
        private double HexWidth => Settings.Density * DipHexWidth;
        private double HexHeight => Settings.Density * DipHexHeight;

        private Rectangle RealRect { get; set; }

        private readonly Dictionary<char, SKPath> hexagons = new Dictionary<char, SKPath>();
        private char selectedSymbol = char.MinValue;

        private readonly List<double> xCoords = new List<double>();
        private readonly List<double> yCoords = new List<double>();

        private readonly SKCanvasView control;

        #endregion //Private variables

        public GlossaryAlphabetControl()
        {
            control = new SKCanvasView();

            control.PaintSurface += ControlOnPaintSurface;
            control.EnableTouchEvents = true;
            control.Touch += ControlOnTouch;

            Content = control;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var hCoeff = Height / BaseHeight;
            var wCoeff = Width / BaseWidth;
            var coeff = Math.Min(wCoeff, hCoeff);
            sizeCoefficient = Math.Min(Math.Max(coeff, sizeCoefficient), MaxCoefficient);
            RealRect = new Rectangle(new Point(0, 0), new Size(HexWidth * 6 + D * 5, HexHeight * 7 + D * 8));
            var clipBounds = e.Surface.Canvas.DeviceClipBounds;
            var clipRect = new SKRect(clipBounds.Left, clipBounds.Top, clipBounds.Right, clipBounds.Bottom);
            RealRect = clipRect.ToFormsRect().LocateInCenter(RealRect);
            CreateCoordinates();
            DrawHexagons(e.Surface.Canvas);
        }

        private void CreateCoordinates()
        {
            xCoords.Clear();
            yCoords.Clear();
            for (var i = 0; i < 11; i++)
            {
                xCoords.Add(RealRect.X + (HexWidth + D) * i / 2);
            }
            for (var i = 0; i < 9; i++)
            {
                yCoords.Add(RealRect.Y + (HexHeight * 0.75 + D) * i);
            }
        }

        private void DrawHexagons(SKCanvas canvas)
        {
            hexagons.Clear();

            #region Rects initialization

            var rects = new List<Rectangle>
            {
                //A
                new Rectangle(xCoords[5], yCoords[0], HexWidth, HexHeight),
                //B
                new Rectangle(xCoords[4], yCoords[1], HexWidth, HexHeight),
                //C
                new Rectangle(xCoords[6], yCoords[1], HexWidth, HexHeight),
                //D
                new Rectangle(xCoords[8], yCoords[1], HexWidth, HexHeight),
                //E
                new Rectangle(xCoords[3], yCoords[2], HexWidth, HexHeight),
                //F
                new Rectangle(xCoords[5], yCoords[2], HexWidth, HexHeight),
                //G
                new Rectangle(xCoords[7], yCoords[2], HexWidth, HexHeight),
                //H
                new Rectangle(xCoords[6], yCoords[3], HexWidth, HexHeight),
                //I
                new Rectangle(xCoords[1], yCoords[4], HexWidth, HexHeight),
                //J
                new Rectangle(xCoords[3], yCoords[4], HexWidth, HexHeight),
                //K
                new Rectangle(xCoords[7], yCoords[4], HexWidth, HexHeight),
                //L
                new Rectangle(xCoords[9], yCoords[4], HexWidth, HexHeight),
                //M
                new Rectangle(xCoords[0], yCoords[5], HexWidth, HexHeight),
                //N
                new Rectangle(xCoords[2], yCoords[5], HexWidth, HexHeight),
                //O
                new Rectangle(xCoords[4], yCoords[5], HexWidth, HexHeight),
                //P
                new Rectangle(xCoords[6], yCoords[5], HexWidth, HexHeight),
                //Q
                new Rectangle(xCoords[8], yCoords[5], HexWidth, HexHeight),
                //R
                new Rectangle(xCoords[1], yCoords[6], HexWidth, HexHeight),
                //S
                new Rectangle(xCoords[3], yCoords[6], HexWidth, HexHeight),
                //T
                new Rectangle(xCoords[5], yCoords[6], HexWidth, HexHeight),
                //U
                new Rectangle(xCoords[7], yCoords[6], HexWidth, HexHeight),
                //V
                new Rectangle(xCoords[6], yCoords[7], HexWidth, HexHeight),
                //W
                new Rectangle(xCoords[8], yCoords[7], HexWidth, HexHeight),
                //X
                new Rectangle(xCoords[10], yCoords[7], HexWidth, HexHeight),
                //Y
                new Rectangle(xCoords[7], yCoords[8], HexWidth, HexHeight),
                //Z
                new Rectangle(xCoords[9], yCoords[8], HexWidth, HexHeight)
            };

            #endregion //Rects initialization

            for (var i = 65; i < 91; i++)
            {
                hexagons.Add((char)i, canvas.DrawHexagon(rects[i - 65], i == selectedSymbol ? SelectedFillColor : FillColor, i == selectedSymbol ? SelectedFillColor : FillColor, 0));
                canvas.DrawText(((char)i).ToString(), rects[i - 65], FontColor, FontSize, true);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            //if (DeviceInfo.Platform != DevicePlatform.iOS) return;
            //Invalidate();
            //control.InvalidateSurface();
        }

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(GlossaryAlphabetControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GlossaryAlphabetControl)bindable;
                    ctrl.FillColor = (Color)newValue;
                });

        /// <summary>
        /// Gets or sets the FillColor of the GlossaryAlphabetControl instance.
        /// </summary>
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        /// <summary>
        /// The SelectedFillColor property.
        /// </summary>
        public static BindableProperty SelectedFillColorProperty =
            BindableProperty.Create(nameof(SelectedFillColor), typeof(Color), typeof(GlossaryAlphabetControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GlossaryAlphabetControl)bindable;
                    ctrl.SelectedFillColor = (Color)newValue;
                });

        /// <summary>
        /// Gets or sets the SelectedFillColor of the GlossaryAlphabetControl instance.
        /// </summary>
        public Color SelectedFillColor
        {
            get => (Color)GetValue(SelectedFillColorProperty);
            set => SetValue(SelectedFillColorProperty, value);
        }

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(int), typeof(GlossaryAlphabetControl), 0, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GlossaryAlphabetControl)bindable;
                    ctrl.FontSize = (int)newValue;
                });

        public int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(GlossaryAlphabetControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (GlossaryAlphabetControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        #region Touch event implementation
        protected virtual void RaiseTouchedEvent(char letter)
        {
            Touched?.Invoke(this, new AlphabetControlEventArgs(letter));
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
                    if (selectedSymbol != char.MinValue)
                        RaiseTouchedEvent(selectedSymbol);
                    selectedSymbol = char.MinValue;
                    control.InvalidateSurface();
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
            var element = hexagons.FirstOrDefault(h => h.Value.Contains(point.X, point.Y));

            if (element.Value == null)
                TouchEnd();
            else
            {
                selectedSymbol = element.Key;
                control.InvalidateSurface();
            }
        }

        private void TouchEnd()
        {
            selectedSymbol = char.MinValue;
            control.InvalidateSurface();
        }
        #endregion //Touch event implementation
    }
}
