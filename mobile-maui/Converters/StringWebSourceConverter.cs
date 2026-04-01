using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BioBrain.Converters
{
    public class StringWebSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return value;
            var str = (string) value;
            if(str.Contains("<html>")) return new HtmlWebViewSource {Html = str};
            return new UrlWebViewSource {Url = str};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}