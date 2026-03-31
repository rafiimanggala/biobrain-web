using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Biobrain.Application.Services
{
    internal interface ITemplateService
    {
        string ApplyTemplate(string template, List<TemplateValue> values);
    }

    internal record TemplateValue
    {
	    public string Name { get; init; }
	    public int Index { get; init; }
    }

    internal static class Modificator
    {
	    public static readonly string Index = "i";
        public static readonly string LongIndex = "l";
    }

    internal sealed class TemplateService : ITemplateService
    {
        private const string TemplateItemPatten = "({([0-9])+[:]?([a-z])?})";


        public string ApplyTemplate(string template, List<TemplateValue> values)
        {
	        var regex = new Regex(TemplateItemPatten);
	        var matches = regex.Matches(template);
	        var result = template;

            foreach (var match in matches.OrderByDescending(x => x.Index))
            {
                var indexString = match.Groups[2].Value;
                var modificator = match.Groups[3].Value;
                
                if(!int.TryParse(indexString,out var index)) continue;
                if(values.Count() < index+1) continue;

                result = result.Remove(match.Index, match.Length);
                if (modificator == Modificator.Index)
	                result = result.Insert(match.Index, values[index].Index.ToString());
                else if (modificator == Modificator.LongIndex)
                {
                    var value = "";
                    for (var i = 0; i <= index; i++)
                    {
                        if(i != 0)
                            value += ".";
                        value += values[index].Index.ToString();
                    }
                    result = result.Insert(match.Index, value);
                }
                else
	                result = result.Insert(match.Index, values[index].Name);
            }

            return result;
        }
    }
}
