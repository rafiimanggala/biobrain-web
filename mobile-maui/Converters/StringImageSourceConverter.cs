using System;
using System.Globalization;
using System.IO;
using Microsoft.Maui.Controls;

namespace BioBrain.Converters
{
    public class StringImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return value;
            var str = (string) value;
            return ImageSource.FromResource(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}