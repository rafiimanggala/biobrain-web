using System.Collections.Generic;
using System.Text;

namespace Biobrain.Application.Interfaces.Notifications
{
    internal static class HtmlBodyHelper
    {
        public static void AddParagraph(this StringBuilder b, string text)
        {
            b.Append("<p>");
            b.Append(text);
            b.Append("</p>");
        }        
        
        public static void AddLinksList(this StringBuilder b, IEnumerable<NotificationLink> links)
        {
            b.Append("<ul>");

            foreach (var (text, url) in links) 
                b.Append($"<li><a href='{url.AbsoluteUri}'>{text}</a></li>");

            b.Append("</ul>");
        }
    }
}