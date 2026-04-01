using System.Collections.Generic;
using System.Linq;
using Common;
using CustomControls.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Controls;

namespace CustomControls
{
    /// <summary>
    /// Control for canvas specific cache
    /// </summary>
    public class CachingControl : ContentView
    {
        private static SKTypeface font;
        private static SKTypeface Font => font ?? (font = SKTypeface.FromStream(new SKFileStream("Nunito-Bold.ttf")));
        public CachingControl()
        {
            var control = new SKCanvasView();

            control.PaintSurface += ControlOnPaintSurface;

            Content = control;
        }

        private void ControlOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            //CreateAlphabetControlCache
        }

        //public static Dictionary<string, TextMetrics> AlphabetControlMetrics { get; set; }

        //private static void CreateAlphabetControlCache(SKCanvas canvas)
        //{
        //    if (AlphabetControlMetrics != null && AlphabetControlMetrics.Any()) return;
        //    AlphabetControlMetrics = new Dictionary<string, TextMetrics>();

        //    var paint = new SKPaint
        //    {
        //        TextSize = 15 * Settings.Density,
        //        Typeface = Font
        //    };
        //    for (var i = 65; i < 91; i++)
        //    {
        //        var text = ((char)i).ToString();
        //        AlphabetControlMetrics.Add(text, paint.MeasureText(text));
        //    }
        //}
    }
}
