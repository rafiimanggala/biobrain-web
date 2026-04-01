using SkiaSharp;
using Rectangle = Microsoft.Maui.Graphics.Rect;

namespace CustomControls.Extentions
{
    public static class SKRectExtensions
    {
        /// <summary>
        /// Converts SKRect to MAUI Rect (replacement for Xamarin's ToFormsRect).
        /// </summary>
        public static Rectangle ToFormsRect(this SKRect skRect)
        {
            return new Rectangle(skRect.Left, skRect.Top, skRect.Width, skRect.Height);
        }

        /// <summary>
        /// Converts SKRectI to MAUI Rect (replacement for Xamarin's ToFormsRect on SKRectI).
        /// </summary>
        public static Rectangle ToFormsRect(this SKRectI skRect)
        {
            return new Rectangle(skRect.Left, skRect.Top, skRect.Width, skRect.Height);
        }
    }

    public static class RectExtension
    {
        //public static Rect LocateInCenter(this Rect rect, Rect rectToLocate)
        //{
        //    var x = (rect.Width - rectToLocate.Width) / 2;
        //    var y = (rect.Height - rectToLocate.Height) / 2;
        //    rectToLocate.X = x > 0 ? x : 0;
        //    rectToLocate.Y = y > 0 ? y : 0;
        //    return rectToLocate;
        //}

        //public static Rect FixForBorder(this Rect rect, float border)
        //{
        //    return new Rect(rect.X+border, rect.Y + border, rect.Width - border*2, rect.Height - border*2);
        //}

        public static Rectangle LocateInCenter(this Rectangle rect, Rectangle rectToLocate)
        {
            var x = (rect.Width - rectToLocate.Width) / 2;
            var y = (rect.Height - rectToLocate.Height) / 2;
            rectToLocate = new Rectangle(x > 0 ? x : 0, y > 0 ? y : 0, rectToLocate.Width, rectToLocate.Height);
            return rectToLocate;
        }

        public static Rectangle LocateInCenterHorizontal(this Rectangle rect, Rectangle rectToLocate)
        {
            var x = (rect.Width - rectToLocate.Width) / 2;
            rectToLocate = new Rectangle(x > 0 ? x : 0, rectToLocate.Y, rectToLocate.Width, rectToLocate.Height);
            return rectToLocate;
        }

        public static Rectangle LocateInCenterVertical(this Rectangle rect, Rectangle rectToLocate)
        {
            var y = (rect.Height - rectToLocate.Height) / 2;
            rectToLocate = new Rectangle(rectToLocate.X, y > 0 ? y : 0, rectToLocate.Width, rectToLocate.Height);
            return rectToLocate;
        }

        public static Rectangle FixForBorder(this Rectangle rect, float border)
        {
            return new Rectangle(rect.X+border, rect.Y + border, rect.Width - border*2, rect.Height - border*2);
        }
    }
}
