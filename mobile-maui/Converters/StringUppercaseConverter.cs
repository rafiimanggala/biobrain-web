using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BioBrain.Converters
{
    class StringUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).ToLower();
        }
    }
}
