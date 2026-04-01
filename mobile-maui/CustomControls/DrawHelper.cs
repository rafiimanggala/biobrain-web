using System.Collections.Generic;
using System.IO;
using BioBrain.AppResources;
using Common;
using Common.Interfaces;
using Common.Models;
using CustomControls.Extentions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Microsoft.Maui.Controls;
using Color = System.Drawing.Color;

namespace CustomControls
{
    public static class DrawHelper
    {
        public static string DrawTextToBase64(string text)
        {
            using (var textPaint = new SKPaint {TextSize = 48, Typeface = SKTypeface.Default})
            {
                var bounds = new SKRect();
                textPaint.MeasureText(text, ref bounds);
                var textBitmap = new SKBitmap((int) bounds.Right,
                    (int) bounds.Height);
                using (var bitmapCanvas = new SKCanvas(textBitmap) )
                {
                    bitmapCanvas.Clear();
                    bitmapCanvas.DrawText(text, 0, -bounds.Top, textPaint);

                    var base64 = System.Convert.ToBase64String(SKImage.FromBitmap(textBitmap)
                        .Encode(SKEncodedImageFormat.Png, 0).ToArray());
                    return base64;
                }
            }
        }

        private const int Coefficient = 4;
        private static readonly int ImageWidth = 512 * Coefficient;
        private static readonly int Margin = 10 * Coefficient;
        private static readonly int SmallFont = 14 * Coefficient;
        private static readonly int HeaderFont = 32 * Coefficient;
        private static readonly int RowHeight = 60 * Coefficient;
        private static readonly int HeaderHeight = 50 * Coefficient;

        public static AttachmentModel DrawResults(string header, List<IStatEntryViewModel> results)
        {

            var height = HeaderHeight + RowHeight * results.Count;
            var bitmap = new SKBitmap(ImageWidth, height);
            var bitmapCanvas = new SKCanvas(bitmap);
            bitmapCanvas.Clear();

            bitmapCanvas.DrawRect(0,0,ImageWidth, HeaderHeight, new SKPaint {Color = CustomColors.DarkMain.ToSKColor(), Style = SKPaintStyle.Fill });
            bitmapCanvas.DrawText(header, ImageWidth/2, Margin + 28 * Coefficient,
                new SKPaint
                {
                    TextSize = HeaderFont,
                    Typeface = SKTypeface.FromStream(new SKFileStream("Nunito-Bold.ttf")),
                    Color = Color.White.ToSkColor(),
                    FilterQuality = SKFilterQuality.High,
                    TextAlign = SKTextAlign.Center
                });

            for (var i = 0; i < results.Count; i++)
            {
                bitmapCanvas.DrawRect(0, HeaderHeight + RowHeight * i, ImageWidth, HeaderHeight + RowHeight * (i+1), new SKPaint { Color = Color.White.ToSkColor(), Style = SKPaintStyle.Fill });
                var secondLine = Margin + SmallFont + HeaderHeight + RowHeight * i;
                bitmapCanvas.DrawText($"{StringResource.Topic}: {results[i].Topic}", Margin, secondLine,
                    new SKPaint
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        TextSize = SmallFont,
                        Typeface = SKTypeface.FromStream(new SKFileStream("Roboto-Light.ttf")),
                        FakeBoldText = true,
                        Color = CustomColors.DarkMain.ToSKColor(),
                        FilterQuality = SKFilterQuality.High,
                    });

                var thirdLine = secondLine + SmallFont + Margin;
                bitmapCanvas.DrawText($"{StringResource.Level}: {results[i].Level}", Margin, thirdLine,
                    new SKPaint
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        TextSize = SmallFont,
                        Typeface = SKTypeface.FromStream(new SKFileStream("Roboto-Light.ttf")),
                        Color = CustomColors.DarkMain.ToSKColor(),
                        FilterQuality = SKFilterQuality.High,
                    });
                bitmapCanvas.DrawText($"{StringResource.Score}: {results[i].Score}", ImageWidth/2, thirdLine,
                    new SKPaint
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        TextSize = SmallFont,
                        TextAlign = SKTextAlign.Center,
                        Typeface = SKTypeface.FromStream(new SKFileStream("Roboto-Light.ttf")),
                        Color = CustomColors.DarkMain.ToSKColor(),
                        FilterQuality = SKFilterQuality.High,
                    });
                bitmapCanvas.DrawText($"{StringResource.DateCompleted}: {results[i].Date}", ImageWidth - Margin, thirdLine,
                    new SKPaint
                    {
                        Style = SKPaintStyle.StrokeAndFill,
                        TextSize = SmallFont,
                        TextAlign = SKTextAlign.Right,
                        Typeface = SKTypeface.FromStream(new SKFileStream("Roboto-Light.ttf")),
                        Color = CustomColors.DarkMain.ToSKColor(),
                        FilterQuality = SKFilterQuality.High,
                    });
                bitmapCanvas.DrawLine(Margin, thirdLine+Margin, ImageWidth-Margin, thirdLine+Margin, new SKPaint { Color = CustomColors.DarkMain.ToSKColor(),
                    Style = SKPaintStyle.Stroke, StrokeWidth = 3});
            }

            var base64 = System.Convert.ToBase64String(SKImage.FromBitmap(bitmap)
                .Encode(SKEncodedImageFormat.Png, 100).ToArray());

            // TODO: DependencyService not available in MAUI - replace with MAUI dependency injection
            var paths = DependencyService.Get<IFilesPath>();
            var imgPath = Path.Combine(paths.InternetCache, Settings.AttachmentFileName);
            if (!Directory.Exists(paths.InternetCache)) Directory.CreateDirectory(paths.InternetCache);
            if(File.Exists(imgPath)) File.Delete(imgPath);
            using (var stream = File.OpenWrite(imgPath))
            {
                SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            return new AttachmentModel { Base64Image = base64, ImagePath = imgPath };
        }
    }
}
