using System.Linq;
using Microsoft.Maui.Controls;

namespace CustomControls.Effects
{
    // TODO: RoutingEffect is not available in MAUI.
    // In MAUI, use Platform Behaviors or the built-in Border control instead.
    // The EntryBorderEffect and its RoutingEffect base class need to be replaced
    // with a MAUI-compatible approach (e.g., Handler customization or Border control).
    public class BorderEffect
    {
        static void OnHasNoBorderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            var hasNoBorder = (bool)newValue;
            if (hasNoBorder)
            {
                // TODO: Replace with MAUI Platform Behavior or Handler customization
                // view.Effects.Add(new EntryBorderEffect());
            }
            else
            {
                // TODO: Replace with MAUI Platform Behavior or Handler customization
                // var toRemove = view.Effects.FirstOrDefault(e => e is EntryBorderEffect);
                // if (toRemove != null)
                // {
                //     view.Effects.Remove(toRemove);
                // }
            }
        }

        // TODO: RoutingEffect removed in MAUI - replace with PlatformBehavior<T> or Handler customization
        // class EntryBorderEffect : RoutingEffect
        // {
        //     public EntryBorderEffect() : base("andapex.EntryBorderEffect")
        //     {
        //     }
        // }

        public static readonly BindableProperty HasNoBorderProperty = BindableProperty.Create(
            propertyName: "HasNoBorder",
            returnType: typeof(bool),
            declaringType: typeof(BorderEffect),
            defaultValue: default(bool),
            propertyChanged: OnHasNoBorderChanged);

        public static bool GetHasNoBorder(BindableObject obj)
        {
            return (bool)obj.GetValue(HasNoBorderProperty);
        }

        public static void SetHasNoBorder(BindableObject obj, bool value)
        {
            obj.SetValue(HasNoBorderProperty, value);
        }
    }
}
