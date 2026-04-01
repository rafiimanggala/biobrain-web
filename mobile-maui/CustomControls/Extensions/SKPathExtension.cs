using SkiaSharp;

namespace CustomControls.Extentions
{
    public static class SKPathExtension
    {
        public static SKPath MoveTo2(this SKPath path, SKPoint point)
        {
            path.MoveTo(point);
            return path;
        }
        public static SKPath LineTo2(this SKPath path, SKPoint point)
        {
            path.LineTo(point);
            return path;
        }
        public static SKPath ArcTo2(this SKPath path, SKPoint angle, SKPoint destination, long radius)
        {
            path.ArcTo(angle, destination, radius);
            return path;
        }
    }
}
