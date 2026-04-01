using SkiaSharp;
using Microsoft.Maui.Graphics;

namespace CustomControls.Extentions
{
    public static class ColorExtension
    {
        //public static NGraphics.Color ToNgraphicsColor(this Color color)
        //{
        //    return NGraphics.Color.FromRGB(color.R, color.G, color.B);
        //}
        public static SKColor ToSkColor(this System.Drawing.Color color)
        {
            return new SKColor((byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}
