using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using BioBrain.ViewModels.Implementation;
using Common;
using Common.Interfaces;
using Microsoft.Maui.Controls;
// using Xamarin.Forms.Internals; // TODO: Review MAUI equivalent

namespace BioBrain.Helpers
{
    public static class ResponseHelper
    {
        private static Dictionary<string, string> defaultValues => new Dictionary<string, string>
            {
                {"Error", "Unexpected value during parsing responce url:"}
            };

        static readonly IErrorLog logger = DependencyService.Get<IErrorLog>();

        public static WebViewResponseModel GetResponseForMaterials(string url)
        {
            try
            {
                var startIndex = url.LastIndexOf('/');

                if (startIndex == -1)
                {
                    logger.Log($"Url1:{url}");
                    return new WebViewResponseModel { Type = ResponseTypes.Error, ParsedResponse = defaultValues };
                }
                url = Regex.Replace(url.Substring(startIndex + 1), @"\t|\n|\r", "");

                var result = new WebViewResponseModel();

                if (url.StartsWith("glossary."))
                {
                    var args = Regex.Replace(WebUtility.UrlDecode(url), @"\t|\n|\r", "");
                    result.Type = ResponseTypes.Glossary;
                    var reference = args.Replace("glossary.", string.Empty);
                    result.ParsedResponse.Add("glossary", $"'{reference}'");
                    return result;
                }

                if (url.StartsWith("material."))
                {
                    var args = Regex.Replace(WebUtility.UrlDecode(url), @"\t|\n|\r", "");
                    result.Type = ResponseTypes.Material;
                    var reference = args.Replace("material.", string.Empty).Split('.');
                    result.ParsedResponse.Add("topic", $"{reference[0]}");
                    result.ParsedResponse.Add("level", $"{reference[1]}");
                    return result;
                }

                if (url.StartsWith("area."))
                {
                    var args = Regex.Replace(WebUtility.UrlDecode(url), @"\t|\n|\r", "");
                    result.Type = ResponseTypes.Area;
                    var reference = args.Replace("area.", string.Empty);
                    result.ParsedResponse.Add("area", $"{reference}");
                    return result;
                }

                if (url.StartsWith("m.html?"))
                {
                    var answers = url.Substring(7).Split('&');

                    result.Type = answers.Count() > 1 ? ResponseTypes.MultiAnswer : ResponseTypes.SingleAnswer;

                    var i = 0;
                    foreach (var answer in answers)
                    {
                        var decodedAnswer = Regex.Replace(answer, @"\t|\n|\r", "");
                        var index = decodedAnswer.IndexOf('=');
                        if (index == -1)
                        {
                            logger.Log($"Url2:{url}");
                            result.ParsedResponse.Add((i+1).ToString(), decodedAnswer);
                        }
                        else
                            result.ParsedResponse.Add(decodedAnswer.Substring(0, index), WebUtility.UrlDecode(decodedAnswer.Substring(index + 1)));
                        i++;
                    }

                    return result;
                }
                logger.Log($"Url3:{url}");
                return new WebViewResponseModel { Type = ResponseTypes.Error, ParsedResponse = defaultValues };
            }
            catch (Exception)
            {
                logger.Log($"Url4:{url}");
                throw;
            }
        }

        public static string GetResponseTextUrls(string url)
        {
            const string glossaryPattern = @"file.*\/\@(.*)";
            var reg = new Regex(glossaryPattern);
            var match = reg.Match(url);
            if (match.Groups.Count < 2) return null;
            return match.Groups[1]?.Captures[0].Value;
        }
    }
}