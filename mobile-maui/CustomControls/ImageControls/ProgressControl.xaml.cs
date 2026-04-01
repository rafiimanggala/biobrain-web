using System;
using Microsoft.Maui.Controls;

namespace CustomControls.ImageControls
{
    public partial class ProgressControl : ContentView
    {
        public ProgressControl()
        {
            InitializeComponent();
        }

        public static BindableProperty ProgressValueProperty =
           BindableProperty.Create(nameof(ProgressValue), typeof(int), typeof(ProgressControl), -1, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (ProgressControl)bindable;
                   ctrl.ProgressValue = (int)newValue;
               });

        public int ProgressValue
        {
            get { return (int)GetValue(ProgressValueProperty); }
            set
            {
                SetValue(ProgressValueProperty, value);
                if (value < 0)
                {
                    SymbolImage.Source = ImageSource.FromFile("downarrowicon.png");
                }
                else if (value > 0)
                {
                    SymbolImage.Source = ImageSource.FromFile("uparrowicon.png");
                }
                else
                {
                    SymbolImage.Source = ImageSource.FromFile("updownarrowicon.png");
                }
                ValueLabel.Text = Math.Abs(value) + "%";
            }
        }
    }
}
