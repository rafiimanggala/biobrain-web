using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BioBrain.Effects
{
    // TODO: RoutingEffect is not available in MAUI.
    // In MAUI, use the built-in Border control with RoundRectangle StrokeShape instead.
    // This stub allows XAML compilation; the actual rounding effect is a no-op.

    /// <summary>
    /// Stub for the original Xamarin.Forms RoutingEffect-based RoundedCorners.
    /// In MAUI, prefer using Border with RoundRectangle StrokeShape instead.
    /// </summary>
    public class RoundedCorners : RoutingEffect
    {
    }

    public static class RoundedCornerProperties
    {
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.CreateAttached("BorderColor", typeof(Color), typeof(RoundedCornerProperties), Colors.Transparent);

        public static Color GetBorderColor(BindableObject view)
        {
            return (Color)view.GetValue(BorderColorProperty);
        }

        public static void SetBorderColor(BindableObject view, Color value)
        {
            view.SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BorderThicknessProperty =
            BindableProperty.CreateAttached("BorderThickness", typeof(double), typeof(RoundedCornerProperties), 0.0);

        public static double GetBorderThickness(BindableObject view)
        {
            return (double)view.GetValue(BorderThicknessProperty);
        }

        public static void SetBorderThickness(BindableObject view, double value)
        {
            view.SetValue(BorderThicknessProperty, value);
        }

        public static readonly BindableProperty RoundRadiusProperty =
            BindableProperty.CreateAttached("RoundRadius", typeof(double), typeof(RoundedCornerProperties), 0.0);

        public static double GetRoundRadius(BindableObject view)
        {
            return (double)view.GetValue(RoundRadiusProperty);
        }

        public static void SetRoundRadius(BindableObject view, double value)
        {
            view.SetValue(RoundRadiusProperty, value);
        }
    }
}
