using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls
{
    public class CustomCell : ViewCell
    {
        /// <summary>
        /// The FillColor property.
        /// </summary>
        public static BindableProperty SelectedFillColorProperty =
            BindableProperty.Create(nameof(SelectedFillColor), typeof(Color), typeof(CustomCell),
                Colors.Gray, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (CustomCell)bindable;
                    ctrl.SelectedFillColor = (Color)newValue;
                });

        /// <summary>
        /// Gets or sets the FillColor of the CircularButtonControl instance.
        /// </summary>
        /// <value>The color of the buton.</value>
        public Color SelectedFillColor
        {
            get { return (Color)GetValue(SelectedFillColorProperty); }
            set
            {
                SetValue(SelectedFillColorProperty, value);
            }
        }
    }
}
