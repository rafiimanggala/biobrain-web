using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Common.Interfaces;
using CustomControls.Extensions;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Point = Microsoft.Maui.Graphics.Point;
using Rect = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.LayoutControls.ChemicalElements
{
    /// <summary>
    /// Chemical elements periodic table control rendered via SkiaSharp.
    /// Displays the full periodic table with interactive element selection popup.
    /// </summary>
    public class ChemicalElementsControl : ContentView
    {
        private readonly IErrorLog logger = DependencyService.Get<IErrorLog>();
        public event EventHandler Touched;
        public event EventHandler CrossTouched;

        #region Private variables

        private const string ChemistryIconId = "BioBrain.CustomControls.LayoutControls.ChemicalElements.InappIcon.png";
        private const string CrossIconId = "BioBrain.CustomControls.LayoutControls.ChemicalElements.HintCross.png";

        // Default values
        private const float DefaultRectHeight = 28f;
        private const int DefaultFontSize = 16;

        // Dip values
        private static float dipRectHeight = DefaultRectHeight;
        private const float DipD = 2;
        private const float DipHeaderHeight = 40f;

        // Scale factor
        private static float ScaleFactor => dipRectHeight / DefaultRectHeight;

        // Density helper
        private static float Density => (float)DeviceDisplay.MainDisplayInfo.Density;

        // Values in pixels for drawing
        private static float D => Density * DipD;
        private static float RectHeight => Density * dipRectHeight;
        private static float HeaderHeight => Density * DipHeaderHeight;
        private static float FontSize => DefaultFontSize * ScaleFactor * Density;
        private static float SmallFontSize => FontSize * 0.6f;
        private static float MediumFontSize => FontSize * 0.8f;
        private static float PopupHeight => RectHeight * 2.75f;
        private static float PopupLegendHeight => RectHeight * 2.25f;
        private static float PopupLettersFont => DeviceInfo.Platform == DevicePlatform.iOS ? FontSize * 1.5f : FontSize * 1.1f;
        private static float PopupNumberFont => SmallFontSize * 1.65f;
        private static float PopupNameFont => SmallFontSize * 1.3f;

        private const float HorizontalNumberOfCells = 18;
        private const float VerticalNumberOfCells = 9;

        // Rect to draw
        private Rect realRect = new Rect(
            new Point(0, 0),
            new Microsoft.Maui.Graphics.Size(
                RectHeight * HorizontalNumberOfCells + D * (HorizontalNumberOfCells - 1),
                RectHeight * VerticalNumberOfCells + D * (VerticalNumberOfCells + 3)));

        // Element rects (to find clicked)
        private readonly Dictionary<string, Rect> rectangles = new Dictionary<string, Rect>();
        private Rect crossRect = new Rect();

        // Selected element
        private string selectedSymbol = string.Empty;

        // Grid coordinates
        private readonly List<float> xCoords = new List<float>();
        private readonly List<float> yCoords = new List<float>();

        private readonly SKCanvasView control;

        // Cached images
        private SKBitmap chemistryIconBitmap;
        private SKBitmap crossIconBitmap;

        #endregion // Private variables

        public ChemicalElementsControl()
        {
            control = new SKCanvasView();
            control.PaintSurface += ControlOnPaintSurface;
            control.EnableTouchEvents = true;
            control.Touch += ControlOnTouch;
            Content = control;

            LoadEmbeddedImages();
        }

        private void LoadEmbeddedImages()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            chemistryIconBitmap = LoadBitmapFromResource(assembly, ChemistryIconId);
            crossIconBitmap = LoadBitmapFromResource(assembly, CrossIconId);
        }

        private static SKBitmap LoadBitmapFromResource(Assembly assembly, string resourceId)
        {
            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceId);
                if (stream == null) return null;
                return SKBitmap.Decode(stream);
            }
            catch
            {
                return null;
            }
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            CalcSizes(e.Surface.Canvas.DeviceClipBounds.ToFormsRect());
            realRect = e.Surface.Canvas.DeviceClipBounds.ToFormsRect().LocateInCenter(realRect);
            CalcGridCoordinatesInternal();
            DrawElements(e.Surface.Canvas);
            DrawLegend(e.Surface.Canvas);
            DrawIcon(e.Surface.Canvas);
            DrawHeader(e.Surface.Canvas);
            if (!string.IsNullOrEmpty(selectedSymbol))
                DrawPopup(e.Surface.Canvas);
        }

        private void CalcSizes(Rect canvasRect)
        {
            var canvasRectDipHeight = (float)canvasRect.Height / Density;
            var dipWidthParent = ((View)Parent).Width - Margin.Left - Margin.Right;

            var vertikalKoef = 1;
            var horizontalKoef = 1.3;
            // (numberofcells + koef) - Add x of cell height to horizontal and vertical size for annotation
            var newByHeight = (canvasRectDipHeight - DipD * (VerticalNumberOfCells + 5) - DipHeaderHeight) / (VerticalNumberOfCells + vertikalKoef);
            var newByWidth = (dipWidthParent - DipD * (HorizontalNumberOfCells - 1)) / (HorizontalNumberOfCells + horizontalKoef);
            dipRectHeight = (float)Math.Round(newByWidth < newByHeight ? newByWidth : newByHeight);
            control.WidthRequest = dipRectHeight * (HorizontalNumberOfCells + horizontalKoef) + DipD * (HorizontalNumberOfCells - 1);
            control.HeightRequest = dipRectHeight * (VerticalNumberOfCells + vertikalKoef) + DipD * (VerticalNumberOfCells + 5);
            realRect = new Rect(
                new Point(0, HeaderHeight),
                new Microsoft.Maui.Graphics.Size(
                    RectHeight * (HorizontalNumberOfCells + horizontalKoef) + D * (HorizontalNumberOfCells - 1),
                    RectHeight * (VerticalNumberOfCells + vertikalKoef) + D * (VerticalNumberOfCells + 5)));
        }

        private void CalcGridCoordinatesInternal()
        {
            xCoords.Clear();
            yCoords.Clear();
            for (var i = 0; i < 18; i++)
                xCoords.Add((float)realRect.X + (RectHeight + D) * i + RectHeight * 1.1f);
            for (var i = 0; i < 9; i++)
                yCoords.Add((float)(realRect.Y + (RectHeight + D) * i) + (i > 6 ? D * 5 : 0) + RectHeight * 0.65f);
        }

        private void DrawElements(SKCanvas canvas)
        {
            canvas.Clear();
            rectangles.Clear();

            foreach (var rect in ChemicalElementsData.ChemicalElements)
            {
                var element = rect.Value;
                var rectangle = new Rect(xCoords[element.X], yCoords[element.Y], RectHeight, RectHeight);
                rectangles.Add(rect.Key, rectangle);

                canvas.DrawRectangle(rectangle, rect.Key == selectedSymbol ? SelectedFillColor : element.BackgroundColor);
                if (rect.Value.FontColor != Colors.White)
                    canvas.DrawStrokeRectangle(rectangle, element.FontColor);
                canvas.DrawTextOnBottomCenter(element.ShortName, rectangle, element.FontColor, FontSize, D * ScaleFactor);
                canvas.DrawTextOnTopLeft(element.AtomicNumber.ToString(), rectangle, element.FontColor, SmallFontSize, D);
                if (element.IsGroupFirst)
                    canvas.DrawTextAbove((element.X + 1).ToString(), rectangle, CustomColors.DarkGray, SmallFontSize, D);
                if (element.IsPeriodFirsr)
                    canvas.DrawTextLeft((element.Y + 1).ToString(), rectangle, CustomColors.DarkGray, SmallFontSize, D,
                        (element.Y + 1) == 1 ? Microsoft.Maui.TextAlignment.End : Microsoft.Maui.TextAlignment.Center);
                if (!string.IsNullOrEmpty(element.BlockName))
                {
                    if (element.BlockName == StringResource.FBlockString)
                    {
                        canvas.DrawTextAbove(element.BlockName, rectangle, CustomColors.DarkGray, SmallFontSize, D);
                    }
                    else
                    {
                        canvas.DrawTextTwoLineAbove(element.BlockName, rectangle, CustomColors.DarkGray, SmallFontSize, D);
                    }
                }
            }
        }

        private void DrawLegend(SKCanvas canvas)
        {
            canvas.DrawGroupsPeriods(new Rect(xCoords[0], yCoords[0], RectHeight, RectHeight), CustomColors.DarkGray, SmallFontSize, D);
            canvas.DrawBlocks(new Rect(xCoords[5], yCoords[1], RectHeight, RectHeight), CustomColors.DarkGray, MediumFontSize, D, RectHeight / 2);
            canvas.DrawPopupLegend(new SKPoint(xCoords[9] + RectHeight / 2, yCoords[1] - RectHeight), CustomColors.ElementDarkGreen, SmallFontSize, MediumFontSize, D, PopupLegendHeight);
        }

        private void DrawHeader(SKCanvas canvas)
        {
            var canvasWidth = (float)canvas.DeviceClipBounds.ToFormsRect().Width;
            logger?.Log($"Draw Periodic Header PopupLettersFont:{PopupLettersFont}, FontSize:{FontSize}, Density:{Density}, Coef:{ScaleFactor}, dipRectHeight:{dipRectHeight}");
            canvas.DrawHeader(HeaderText, new SKPoint(canvasWidth / 2, HeaderHeight), PopupLettersFont, CustomColors.ElementDarkGreen);

            crossRect = new Rect(canvasWidth - (DipHeaderHeight + 10) * Density, 0, HeaderHeight, HeaderHeight);
            if (crossIconBitmap != null)
                canvas.DrawImage(SKImage.FromBitmap(crossIconBitmap), crossRect.ToSKRect());
        }

        private void DrawIcon(SKCanvas canvas)
        {
            var imageSize = (int)(RectHeight * 2 + D);
            if (chemistryIconBitmap != null)
                canvas.DrawImage(SKImage.FromBitmap(chemistryIconBitmap),
                    new SKRect(xCoords[2] + RectHeight / 2, yCoords[1] - imageSize, xCoords[2] + RectHeight / 2 + imageSize, yCoords[1]));
        }

        private void DrawPopup(SKCanvas canvas)
        {
            if (!rectangles.ContainsKey(selectedSymbol)) return;

            var selectedRect = rectangles[selectedSymbol];
            var tableRect = canvas.DeviceClipBounds.ToFormsRect();
            var popupRect = new Rect(0, 0, PopupHeight, PopupHeight);

            // If can draw left - draw left
            if (tableRect.Contains(new Point(selectedRect.Left - PopupHeight - D, selectedRect.Top)))
                popupRect = new Rect(selectedRect.Left - PopupHeight - D, popupRect.Y, popupRect.Width, popupRect.Height);
            else
                popupRect = new Rect(selectedRect.Right + D, popupRect.Y, popupRect.Width, popupRect.Height);

            // If can draw top - draw top
            if (tableRect.Contains(new Point(popupRect.X, selectedRect.Top - PopupHeight - D)))
                popupRect = new Rect(popupRect.X, selectedRect.Top - PopupHeight - D, popupRect.Width, popupRect.Height);
            else
                popupRect = new Rect(popupRect.X, selectedRect.Bottom + D, popupRect.Width, popupRect.Height);

            canvas.DrawRectangle(popupRect, PopupColor);
            canvas.DrawStrokeRectangle(popupRect, CustomColors.DarkMainGreen);
            canvas.DrawElementCard(ChemicalElementsData.ChemicalElements[selectedSymbol], popupRect, CustomColors.DarkMainGreen,
                PopupLettersFont, PopupNumberFont, PopupNameFont, SmallFontSize, D);
        }

        #region Bindable properties

        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static readonly BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(ChemicalElementsControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.FillColor = (Color)newValue;
                });

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        /// <summary>
        /// The PopupColor property.
        /// </summary>
        public static readonly BindableProperty PopupColorProperty =
            BindableProperty.Create(nameof(PopupColor), typeof(Color), typeof(ChemicalElementsControl),
                Colors.White, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.PopupColor = (Color)newValue;
                });

        public Color PopupColor
        {
            get => (Color)GetValue(PopupColorProperty);
            set => SetValue(PopupColorProperty, value);
        }

        /// <summary>
        /// The Orientation property.
        /// </summary>
        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(ChemicalElementsControl),
                StackOrientation.Vertical, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.Orientation = (StackOrientation)newValue;
                });

        public StackOrientation Orientation
        {
            get => (StackOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// The SelectedFillColor property.
        /// </summary>
        public static readonly BindableProperty SelectedFillColorProperty =
            BindableProperty.Create(nameof(SelectedFillColor), typeof(Color), typeof(ChemicalElementsControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.SelectedFillColor = (Color)newValue;
                });

        public Color SelectedFillColor
        {
            get => (Color)GetValue(SelectedFillColorProperty);
            set => SetValue(SelectedFillColorProperty, value);
        }

        /// <summary>
        /// Font Color property.
        /// </summary>
        public static readonly BindableProperty FontColorProperty =
            BindableProperty.Create(nameof(FontColor), typeof(Color), typeof(ChemicalElementsControl),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.FontColor = (Color)newValue;
                });

        public Color FontColor
        {
            get => (Color)GetValue(FontColorProperty);
            set => SetValue(FontColorProperty, value);
        }

        /// <summary>
        /// Header text property.
        /// </summary>
        public static readonly BindableProperty HeaderTextProperty =
            BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(ChemicalElementsControl),
                string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ChemicalElementsControl)bindable;
                    ctrl.HeaderText = (string)newValue;
                });

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        #endregion // Bindable properties

        #region Touch event implementation

        protected virtual void RaiseTouchedEvent()
        {
            Touched?.Invoke(this, EventArgs.Empty);
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
            var mauiPoint = new Point(point.X, point.Y);

            if (crossRect.Contains(mauiPoint))
            {
                OnCrossTouched();
                return;
            }

            var element = rectangles.FirstOrDefault(h => h.Value.Contains(mauiPoint));

            if (string.IsNullOrEmpty(element.Key))
            {
                TouchEnd();
            }
            else
            {
                selectedSymbol = element.Key;
                control.InvalidateSurface();
            }
        }

        private void TouchEnd()
        {
            selectedSymbol = string.Empty;
            control.InvalidateSurface();
        }

        protected virtual void OnCrossTouched()
        {
            CrossTouched?.Invoke(this, EventArgs.Empty);
        }

        #endregion // Touch event implementation
    }
}
