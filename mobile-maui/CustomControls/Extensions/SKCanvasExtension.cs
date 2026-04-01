using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Common;
using CustomControls.Extentions;
using CustomControls.LayoutControls.ChemicalElements;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Microsoft.Maui.Graphics;
using Point = Microsoft.Maui.Graphics.Point;
using Rectangle = Microsoft.Maui.Graphics.Rect;
using TextAlignment = Microsoft.Maui.TextAlignment;

namespace CustomControls.Extensions
{
    public class TextMetrics
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public static class SKCanvasExtension
    {
        // Font height calculation correction
	    private const int FontHeightDelta = -2;
        private static SKTypeface font;

        private static readonly Dictionary<float, float> FontSizeToFontHeightMapper = new Dictionary<float, float>();

        private static SKTypeface Font => font ?? (font = SKTypeface.FromStream(new SKFileStream("Nunito-Bold.ttf")));

        public static void DrawRectangle(this SKCanvas canvas, Rectangle rect, Color color)
        {
            var paint = new SKPaint
            {
                Color = color.ToSKColor(),
                StrokeCap = SKStrokeCap.Round,
                Style = SKPaintStyle.Fill,

            };
            canvas.DrawRect(rect.ToSKRect(), paint);
        }

        public static void DrawStrokeRectangle(this SKCanvas canvas, Rectangle rect, Color color)
        {
            var paint = new SKPaint
            {
                Color = color.ToSKColor(),
                StrokeCap = SKStrokeCap.Round,
                StrokeWidth = 2,
                Style = SKPaintStyle.Stroke,

            };
            canvas.DrawRect(rect.ToSKRect(), paint);
        }

        public static void DrawTextOnBottomCenter(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, float bottomOffset)
        {
            var paint = new SKPaint
            {
                Color=fontColor.ToSKColor(),
                TextSize = fontSize ,
                Typeface = Font
            };
            var textWidth = paint.MeasureText(text);
            canvas.DrawText(text, new SKPoint((float)rect.Center.X - (textWidth / 2), (float)rect.Bottom - 2 * bottomOffset), paint);
        }

        public static void DrawTextOnTopLeft(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, float offset)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(),
                TextSize = fontSize,
                Typeface = Font
            };

            SetFontHeight(fontSize, paint);

            canvas.DrawText(text, new SKPoint((float)rect.Left + offset, (float)rect.Top + offset + FontSizeToFontHeightMapper[fontSize]), paint);
        }

        public static void DrawTextAbove(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, float offset)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(),
                TextSize = fontSize,
                Typeface = Font
            };

            var textWidth = paint.MeasureText(text);
            canvas.DrawText(text, new SKPoint((float)rect.Center.X - textWidth/2, (float)rect.Top - offset), paint);
        }

        public static void DrawTextTwoLineAbove(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, float offset)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(),
                TextSize = fontSize,
                Typeface = Font
            };
            SetFontHeight(fontSize, paint);
            canvas.DrawTextAbove(text, rect, fontColor, fontSize, offset*2+FontSizeToFontHeightMapper[fontSize]);
        }

        public static void DrawTextLeft(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, float offset, TextAlignment alignment = TextAlignment.Center)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(),
                TextSize = fontSize,
                Typeface = Font
            };

            SetFontHeight(fontSize, paint);
            var periodsWidth = paint.MeasureText(StringResource.PeriodsString);
            var textWidth = paint.MeasureText(text);

            SKPoint point;
            switch (alignment)
            {
                case TextAlignment.Start:
                    point = new SKPoint((float)rect.Left - offset - textWidth / 2 - periodsWidth / 2,
                        (float)rect.Y + FontSizeToFontHeightMapper[fontSize]);
                    break;
                case TextAlignment.Center:
                    point = new SKPoint((float) rect.Left - offset - textWidth/2 - periodsWidth/2,
                        (float) rect.Center.Y + FontSizeToFontHeightMapper[fontSize]/2);
                    break;
                case TextAlignment.End:
                    point = new SKPoint((float)rect.Left - offset - textWidth / 2 - periodsWidth / 2,
                        (float)rect.Bottom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }

            canvas.DrawText(text, point, paint);
        }

        public static void DrawGroupsPeriods(this SKCanvas canvas, Rectangle rect, Color fontColor, float fontSize, float offset)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = fontSize, Typeface = Font
            };

            SetFontHeight(fontSize, paint);
            var groupsWidth = paint.MeasureText(StringResource.GroupsString);
            var periodsWidth = paint.MeasureText(StringResource.PeriodsString);

            canvas.DrawText(StringResource.GroupsString, new SKPoint((float) rect.Left - offset - groupsWidth, (float) rect.Top - offset), paint);
            canvas.DrawText(StringResource.PeriodsString, new SKPoint((float) rect.Left - offset - periodsWidth, (float) rect.Top + FontSizeToFontHeightMapper[fontSize]), paint);
            DrawArrows(canvas, rect, fontColor, fontSize, offset, periodsWidth);
        }

        private static void DrawArrows(SKCanvas canvas, Rectangle rect, Color color, float fontSize, float offset, float periodsWidth)
        {
            var arrowPaint = new SKPaint
            {
                Color = color.ToSKColor(),
                TextSize = fontSize,
                StrokeCap = SKStrokeCap.Round,
                StrokeWidth = 2
            };

            var arrowSize = (float)rect.Width / 3 - offset * 2;
            var arrowEndSize = arrowSize / 2f;
            var arrowOffset = FontSizeToFontHeightMapper[fontSize]*0.25f;

            var verticalEnd = new SKPoint((float) rect.Left - periodsWidth/2 - offset,
                (float) rect.Top + FontSizeToFontHeightMapper[fontSize] + offset + arrowSize);

            var horizontalEnd = new SKPoint((float) (rect.Left + arrowSize),
                (float) rect.Top - FontSizeToFontHeightMapper[fontSize]/2);

            //Vertical arrow
            canvas.DrawLine(new SKPoint((float)rect.Left - periodsWidth/2 - offset, (float)rect.Top + FontSizeToFontHeightMapper[fontSize] + offset),
                verticalEnd, arrowPaint);
            canvas.DrawLine(verticalEnd,
                 new SKPoint((float)rect.Left - periodsWidth / 2 - offset+ arrowOffset, (float)rect.Top + FontSizeToFontHeightMapper[fontSize] + offset + arrowSize - arrowEndSize), arrowPaint);
            canvas.DrawLine(verticalEnd,
                new SKPoint((float)rect.Left - periodsWidth / 2 - offset- arrowOffset, (float)rect.Top + FontSizeToFontHeightMapper[fontSize] + offset + arrowSize - arrowEndSize), arrowPaint);

            //Horizontal arrow
            canvas.DrawLine(new SKPoint((float)rect.Left, (float)rect.Top - FontSizeToFontHeightMapper[fontSize] / 2),
                horizontalEnd, arrowPaint);
            canvas.DrawLine(horizontalEnd,
                new SKPoint((float)(rect.Left + arrowSize - arrowEndSize), (float)rect.Top - FontSizeToFontHeightMapper[fontSize] * 0.25f), arrowPaint);
            canvas.DrawLine(horizontalEnd,
                new SKPoint((float)(rect.Left + arrowSize - arrowEndSize), (float)rect.Top - FontSizeToFontHeightMapper[fontSize] * 0.75f), arrowPaint);
        }

        public static void DrawBlocks(this SKCanvas canvas, Rectangle rect, Color fontColor, float fontSize, float offset, float rectHeight)
        {
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = fontSize, Typeface = Font
            };

            SetFontHeight(fontSize, paint);
            var dWidth = paint.MeasureText(StringResource.DBlockString);

            var r = rect.ToSKRect();
            canvas.DrawRect(new SKRect(r.Left, r.Top, r.Left + rectHeight, r.Top + rectHeight), new SKPaint {Color = CustomColors.ElementDarkBlue.ToSKColor(), Style = SKPaintStyle.Fill});
            canvas.DrawRect(new SKRect(r.Left, r.Top + rectHeight + offset, r.Left + rectHeight, r.Top + 2*rectHeight + offset), new SKPaint {Color = CustomColors.ElementLightGreen.ToSKColor(), Style = SKPaintStyle.Fill});
            canvas.DrawRect(new SKRect(r.Left + dWidth + 2*offset + rectHeight, r.Top, r.Left + 2*rectHeight + dWidth + 2*offset, r.Top + rectHeight), new SKPaint {Color = CustomColors.ElementDarkGreen.ToSKColor(), Style = SKPaintStyle.Fill});
            canvas.DrawRect(new SKRect(r.Left + dWidth + 2*offset + rectHeight, r.Top + rectHeight + offset, r.Left + 2*rectHeight + dWidth + 2*offset, r.Top + 2*rectHeight + offset), new SKPaint {Color = CustomColors.ElementLightBlue.ToSKColor(), Style = SKPaintStyle.Fill});

            canvas.DrawText(StringResource.SBlockString, new SKPoint(r.Left + rectHeight + offset, r.Top + rectHeight), paint);
            canvas.DrawText(StringResource.DBlockString, new SKPoint(r.Left + rectHeight + offset, r.Top + 2*rectHeight + offset), paint);
            canvas.DrawText(StringResource.PBlockString, new SKPoint(r.Left + 2*rectHeight + 3*offset + dWidth, r.Top + rectHeight), paint);
            canvas.DrawText(StringResource.FBlockString, new SKPoint(r.Left + 2*rectHeight + 3*offset + dWidth, r.Top + 2*rectHeight + offset), paint);
        }

        public static void DrawPopupLegend(this SKCanvas canvas, SKPoint point, Color color, float smallFontSize, float middleFontSize, float offset, float rectHeight)
        {
            var smallPaint = new SKPaint
            {
                Color = color.ToSKColor(), TextSize = smallFontSize, Typeface = Font
            };
            var middlePaint = new SKPaint
            {
                Color = color.ToSKColor(), TextSize = middleFontSize, Typeface = Font
            };

            SetFontHeight(smallFontSize, smallPaint);
            var symbolWidth = middlePaint.MeasureText(StringResource.SymbolString);
            var nameWidth = smallPaint.MeasureText(StringResource.NameString);
            var massWidth = smallPaint.MeasureText(StringResource.AtomicMassString);
            var strokeWidth = 4;

            canvas.DrawRect(new SKRect(point.X, point.Y, point.X + rectHeight, point.Y + rectHeight), new SKPaint {Color = color.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = strokeWidth });

            //Atomic Number
            canvas.DrawText(StringResource.AtomicString, new SKPoint(point.X + offset + strokeWidth, point.Y + offset + FontSizeToFontHeightMapper[smallFontSize]), smallPaint);
            canvas.DrawText(StringResource.NumberString, new SKPoint(point.X + offset + strokeWidth, point.Y + 2*offset + 2*FontSizeToFontHeightMapper[smallFontSize]), smallPaint);

            //Mass
            var bottomOffset = offset + strokeWidth;
            canvas.DrawText(StringResource.AtomicMassString, new SKPoint(point.X + 0.5f*rectHeight - 0.5f*massWidth, point.Y + rectHeight - bottomOffset), smallPaint);
            //Name
            bottomOffset = bottomOffset + FontSizeToFontHeightMapper[smallFontSize] + offset;
            canvas.DrawText(StringResource.NameString, new SKPoint(point.X + 0.5f*rectHeight - 0.5f*nameWidth, point.Y + rectHeight - bottomOffset), smallPaint);
            //Symbol
            bottomOffset = bottomOffset + FontSizeToFontHeightMapper[smallFontSize] + 4*offset;
            canvas.DrawText(StringResource.SymbolString, new SKPoint(point.X + 0.5f*rectHeight - 0.5f*symbolWidth, point.Y + rectHeight - bottomOffset), middlePaint);
        }

        public static void DrawHeader(this SKCanvas canvas, string text, SKPoint centerPoint, float fontSize, Color color)
        {
            var paint = new SKPaint
            {
                Color = color.ToSKColor(),
                TextSize = fontSize,
                Typeface = Font
            };

            SetFontHeight(fontSize, paint);

            var verticalOffset = (centerPoint.Y - FontSizeToFontHeightMapper[fontSize]) / 2;

            var textWidth = paint.MeasureText(text);

            canvas.DrawText(text, new SKPoint(centerPoint.X - textWidth*0.5f, centerPoint.Y - verticalOffset), paint);
        }

        public static void DrawImage(this SKCanvas canvas, Assembly assembly, string iconId, SKRect rect)
        {
            using (var stream = assembly.GetManifestResourceStream(iconId))
            {
                using (var skStream = new SKManagedStream(stream))
                {
                    var resourceBitmap = SKBitmap.Decode(skStream);
                    canvas.DrawImage(SKImage.FromBitmap(resourceBitmap), rect);
                }
            }
        }

        public static void DrawText(this SKCanvas canvas, string text, Rectangle rect, Color fontColor, float fontSize, bool isAlphabetControl = false)
        {
            var density = (float)DeviceDisplay.MainDisplayInfo.Density;
            var paint = new SKPaint
            {
                Color = fontColor.ToSKColor(),
                TextSize = fontSize * density,
                Typeface = Font
            };

            SetFontHeight(fontSize, paint);
            var txtMeasures = new TextMetrics { Width = paint.MeasureText(text), Height = FontSizeToFontHeightMapper[fontSize] };
            var offset = (rect.Height - txtMeasures.Height) / 2;
            canvas.DrawText(text, new Point(rect.Center.X - (txtMeasures.Width / 2), rect.Y + offset + txtMeasures.Height).ToSKPoint(), paint);
        }

        public static void DrawElementCard(this SKCanvas canvas, IElementViewModel element, Rectangle rect, Color fontColor, float bigFontSize, float numberFontSize, float nameFontSize, float smallFontSize, float offset)
        {
            DrawNumber(canvas, element.AtomicNumber.ToString(), rect, fontColor, numberFontSize, offset);

            DrawWeight(canvas, element.MassNumber.ToString(CultureInfo.InvariantCulture), rect, fontColor, smallFontSize, offset);

            DrawName(canvas, element.Name, rect, fontColor, nameFontSize, offset, FontSizeToFontHeightMapper[smallFontSize]);

            DrawShortName(canvas, element.ShortName, rect, fontColor, bigFontSize, offset, FontSizeToFontHeightMapper[smallFontSize], FontSizeToFontHeightMapper[nameFontSize]);
        }

        public static SKPath DrawHexagon(this SKCanvas canvas, Rectangle rect, Color color, Color borderColor,
            float borderThickness)
        {
            borderColor = borderColor.Alpha == 0.0f ? color : borderColor;
            borderColor = borderThickness == 0.0 ? color : borderColor;
            var p1 = new Point(rect.X, rect.Y + rect.Height * 0.25).ToSKPoint();
            var p2 = new Point(rect.X, rect.Y + rect.Height * 0.75).ToSKPoint();
            var p3 = new Point(rect.X + rect.Width * 0.5, rect.Y + rect.Height).ToSKPoint();
            var p4 = new Point(rect.X + rect.Width, rect.Y + rect.Height * 0.75).ToSKPoint();
            var p5 = new Point(rect.X + rect.Width, rect.Y + rect.Height * 0.25).ToSKPoint();
            var p6 = new Point(rect.X + rect.Width * 0.5, rect.Y).ToSKPoint();

            var path = new SKPath();
            path.MoveTo2(p1).LineTo2(p2).LineTo2(p3).LineTo2(p4).LineTo2(p5).LineTo2(p6).Close();

            // Draw fill
            var paint = new SKPaint {Style = SKPaintStyle.Fill, Color = color.ToSKColor()};
            canvas.DrawPath(path, paint);

            // Draw border
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = borderColor.ToSKColor();
            paint.StrokeWidth = borderThickness;
            canvas.DrawPath(path, paint);

            return path;
        }

        public static SKPath DrawRightRoundedButton(this SKCanvas canvas, string text, int fontSize, Color fontColor, Rectangle rect, long borderWidth, Color borderColor, Color fillColor, long roundRadius)
        {
            //var roundRadius = 0.1*rect.Width;

            var points = new List<SKPoint>
            {
                new Point(rect.X, rect.Y).ToSKPoint(), // 0
                new Point(rect.X + rect.Width - roundRadius, rect.Y).ToSKPoint(),// 1
                new Point(rect.X + rect.Width, rect.Y).ToSKPoint(), // 2 angle
                new Point(rect.X + rect.Width, rect.Y + roundRadius).ToSKPoint(), // 3
                new Point(rect.X + rect.Width, rect.Y + rect.Height - roundRadius).ToSKPoint(), // 4
                new Point(rect.X + rect.Width, rect.Y + rect.Height).ToSKPoint(), // 5 corner
                new Point(rect.X + rect.Width - roundRadius, rect.Y + rect.Height).ToSKPoint(), // 6
                new Point(rect.X, rect.Y + rect.Height).ToSKPoint() // 7
            };

            var path = new SKPath();
            path.MoveTo2(points[0]).LineTo2(points[1]).ArcTo2(points[2], points[3], roundRadius)
                .LineTo2(points[4]).ArcTo2(points[5], points[6], roundRadius)
                .LineTo2(points[7]).Close();

            canvas.DrawPath(path, new SKPaint{Color = fillColor.ToSKColor(), Style = SKPaintStyle.Fill});
            canvas.DrawPath(path, new SKPaint{Color = borderColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = borderWidth});

            canvas.DrawText(text, rect, fontColor, fontSize);

            return path;
        }

        public static SKPath DrawSquareButton(this SKCanvas canvas, string text, int fontSize, Color fontColor, Rectangle rect, long borderWidth, Color borderColor, Color fillColor)
        {
            var rectangle = rect.ToSKRect();
            var path = new SKPath();
            path.MoveTo(rectangle.Left, rectangle.Top);
            path.LineTo(rectangle.Right, rectangle.Top);
            path.LineTo(rectangle.Right, rectangle.Bottom);
            path.LineTo(rectangle.Left, rectangle.Bottom);
            path.Close();

            canvas.DrawPath(path, new SKPaint { Color = fillColor.ToSKColor(), Style = SKPaintStyle.Fill });
            canvas.DrawPath(path, new SKPaint { Color = borderColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = borderWidth });

            canvas.DrawText(text, rect, fontColor, fontSize);

            return path;
        }

        public static SKPath DrawLeftRoundedButton(this SKCanvas canvas, string text, int fontSize, Color fontColor, Rectangle rect, long borderWidth, Color borderColor, Color fillColor, long roundRadius)
        {
            //var roundRadius = 0.1 * rect.Width;

            var points = new List<SKPoint>
            {
                new Point(rect.X + rect.Width, rect.Y + rect.Height).ToSKPoint(), // 0
                new Point(rect.X + roundRadius, rect.Y + rect.Height).ToSKPoint(), // 1
                new Point(rect.X, rect.Y + rect.Height).ToSKPoint(), // 2 corner
                new Point(rect.X, rect.Y + rect.Height - roundRadius).ToSKPoint(), // 3
                new Point(rect.X, rect.Y + roundRadius).ToSKPoint(), // 4
                new Point(rect.X, rect.Y).ToSKPoint(), // 5 corner
                new Point(rect.X + roundRadius, rect.Y).ToSKPoint(), // 6
                new Point(rect.X + rect.Width, rect.Y).ToSKPoint() // 7
            };

            var path = new SKPath();
            path.MoveTo2(points[0]).LineTo2(points[1]).ArcTo2(points[2], points[3], roundRadius)
                .LineTo2(points[4]).ArcTo2(points[5], points[6], roundRadius)
                .LineTo2(points[7]).Close();

            canvas.DrawPath(path, new SKPaint { Color = fillColor.ToSKColor(), Style = SKPaintStyle.Fill });
            canvas.DrawPath(path, new SKPaint { Color = borderColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = borderWidth });

            canvas.DrawText(text, rect, fontColor, fontSize);

            return path;
        }

        public static SKPath DrawBothRoundedButton(this SKCanvas canvas, string text, int fontSize, Color fontColor, Rectangle rect, long borderWidth, Color borderColor, Color fillColor, long roundRadius)
        {
            var points = new List<SKPoint>
            {
                new Point(rect.X + rect.Width - roundRadius, rect.Y + rect.Height).ToSKPoint(), // 0
                new Point(rect.X + roundRadius, rect.Y + rect.Height).ToSKPoint(), // 1
                new Point(rect.X, rect.Y + rect.Height).ToSKPoint(), // 2 corner
                new Point(rect.X, rect.Y + rect.Height - roundRadius).ToSKPoint(), // 3
                new Point(rect.X, rect.Y + roundRadius).ToSKPoint(), // 4
                new Point(rect.X, rect.Y).ToSKPoint(), // 5 corner
                new Point(rect.X + roundRadius, rect.Y).ToSKPoint(), // 6
                new Point(rect.X + rect.Width - roundRadius, rect.Y).ToSKPoint(), // 7
                new Point(rect.X + rect.Width, rect.Y).ToSKPoint(), // 8 corner
                new Point(rect.X + rect.Width, rect.Y + roundRadius).ToSKPoint(), // 9
                new Point(rect.X + rect.Width, rect.Y + rect.Height - roundRadius).ToSKPoint(), // 10
                new Point(rect.X + rect.Width, rect.Y + rect.Height).ToSKPoint() // 11 corner
            };

            var path = new SKPath();
            path.MoveTo2(points[0])
                .LineTo2(points[1]).ArcTo2(points[2], points[3], roundRadius)
                .LineTo2(points[4]).ArcTo2(points[5], points[6], roundRadius)
                .LineTo2(points[7]).ArcTo2(points[8], points[9], roundRadius)
                .LineTo2(points[10]).ArcTo2(points[11], points[0], roundRadius)
                .Close();

            canvas.DrawPath(path, new SKPaint { Color = borderColor.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = borderWidth });
            canvas.DrawPath(path, new SKPaint { Color = fillColor.ToSKColor(), Style = SKPaintStyle.Fill });

            canvas.DrawText(text, rect, fontColor, fontSize);

            return path;
        }

        #region Private methods

        private static void SetFontHeight(float fontSize, SKPaint paint)
        {
            if (!FontSizeToFontHeightMapper.ContainsKey(fontSize))
            {
                paint.GetFontMetrics(out var metric);
                var density = (float)DeviceDisplay.MainDisplayInfo.Density;
                FontSizeToFontHeightMapper.Add(fontSize, -metric.Ascent + FontHeightDelta*density);
            }
        }

        private static void DrawNumber(SKCanvas canvas, string atomicNumber, Rectangle rect, Color fontColor, float numberFontSize, float offset)
        {
            var numberPaint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = numberFontSize, Typeface = Font
            };

            //Atomic number
            SetFontHeight(numberFontSize, numberPaint);
            canvas.DrawText(atomicNumber, new SKPoint((float) rect.Left + offset, (float) rect.Top + offset + FontSizeToFontHeightMapper[numberFontSize]), numberPaint);
        }

        private static void DrawWeight(SKCanvas canvas, string massNumber, Rectangle rect, Color fontColor, float smallFontSize, float offset)
        {
            var smallPaint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = smallFontSize, Typeface = Font
            };

            //weight
            SetFontHeight(smallFontSize, smallPaint);

            var text = $"{massNumber} g/mole";
            var textWidth = smallPaint.MeasureText(text);
            canvas.DrawText(text, new SKPoint((float) rect.Center.X - (textWidth/2), (float) rect.Bottom - 2*offset), smallPaint);
        }

        private static void DrawName(SKCanvas canvas, string name, Rectangle rect, Color fontColor, float nameFontSize, float offset, float weightHeight)
        {
            var namePaint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = nameFontSize, Typeface = Font
            };

            //name
            SetFontHeight(nameFontSize, namePaint);

            var textWidth = namePaint.MeasureText(name);
            canvas.DrawText(name, new SKPoint((float) rect.Center.X - (textWidth/2), (float) rect.Bottom - 4*offset - weightHeight), namePaint);
        }

        private static void DrawShortName(SKCanvas canvas, string shortName, Rectangle rect, Color fontColor, float bigFontSize, float offset, float weightHeight, float nameHeight)
        {
            var bigPaint = new SKPaint
            {
                Color = fontColor.ToSKColor(), TextSize = bigFontSize, Typeface = Font
            };

            //Short name
            var textWidth = bigPaint.MeasureText(shortName);
            canvas.DrawText(shortName, new SKPoint((float) rect.Center.X - (textWidth/2), (float) rect.Bottom - 8*offset - weightHeight - nameHeight), bigPaint);
        }

        #endregion // Private methods
    }
}
