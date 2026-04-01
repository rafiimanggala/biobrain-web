using System;
using System.Globalization;
using Common;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace BioBrain.AppResources
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Text == null ? null : StringResource.ResourceManager.GetString(Text, CultureInfo.CurrentCulture);
        }
    }
}
