using System.Collections.Generic;
using System.IO;
using Biobrain.Application.Common.Models;
using BiobrainWebAPI.Values;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Logging;

namespace Biobrain.Application.Services.Domain.Reports
{
    public class UsageReportPdfService: IUsageReportPdfService
    {
        //private readonly SelectPdf.HtmlToPdf _converter = new();
        private readonly IConverter _converter;
        private readonly ILogger<UsageReportPdfService> _logger;
        private string AppIconPath => Path.Combine(Directory.GetCurrentDirectory(), AppSettings.IconsFolderLink, AppSettings.AppIconLink);

        public UsageReportPdfService(IConverter converter, ILogger<UsageReportPdfService> logger)
        {
            _converter = converter;
            _logger = logger;
        }

        public void GeneratePdf(UsageReportModel model)
        {
            _logger.LogInformation($"Icon path: {AppIconPath}");
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DPI = 75,
                    UseCompression = false,
                    Margins = new MarginSettings(5,5,5,5)
                }
            };
            var html = GetDocument(model);
            doc.Objects.Add(new ObjectSettings
            {
                PagesCount = true,
                WebSettings = { DefaultEncoding = "utf-8" },
                HtmlContent = html
            });
            var bytes = _converter.Convert(doc);
            File.WriteAllBytes(model.ReportPath, bytes);

            _logger.LogTrace($"Report size: {bytes.Length} bytes");
            //var doc = _converter.ConvertHtmlString(html);
            //doc.Save(model.ReportPath);
            //doc.Close();
        }

        private string GetDocument(UsageReportModel model) => @$"
<style>
<link href=""https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,600;0,700;0,800;0,900;1,600;1,700;1,800;1,900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;1,100;1,300;1,400;1,500&display=swap"" rel=""stylesheet"">
.data-table {{
    border: 1px solid black;
  border-collapse: collapse;
        font-family: 'Nunito', sans-serif;
}}
.data-table tr {{
    border: 1px solid black;
  border-collapse: collapse;
}}
.data-table td {{
    border: 1px solid black;
  border-collapse: collapse;
}}
.data-table th {{
    border: 1px solid black;
  border-collapse: collapse;
}}

table{{width: 100%;}}

.data-header{{
    width: 35%; 
    text-align:end; 
    color: #006193; 
    font-weight: bold;
    font-size: 20px;
    font-family: 'Nunito', sans-serif;
}}
.data-entry{{
    width: 65%; 
    text-align:start; 
    padding-left: 10px;
    font-size: 20px;
    font-family: 'Nunito', sans-serif;
}}
</style>
<div class=""container"">
<table>
<tr class=""header"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td style="" font-size: 28px; color: #006193; font-weight: bold; font-family: 'Nunito', sans-serif;"">{model.SchoolName} Usage Report</td>
    <td style=""width: 100px;""><img style=""width: 100px; margin-left:auto; font-family: 'Nunito', sans-serif;"" src=""{AppIconPath}""></td>
</tr>
</table>

<table>
<tr class=""data-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td class=""data-header"">Dates:</td>
    <td class=""data-entry"">{model.FromDate} - {model.ToDate}</td>
</tr>
</table>

<table>
<tr class=""data-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td class=""data-header"">Total number of sessions:</td>
    <td class=""data-entry"">{model.TotalNumberOfSessions}</td>
</tr>
</table>

<table>
<tr class=""data-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td class=""data-header"">Average session duration:</td>
    <td class=""data-entry"">{model.AverageSessionDuration:0.0} minutes</td>
</tr>
</table>

</br>

{(model.SessionsChart.ChartImagePath == string.Empty ? "" 
      : $@"
<table>
<tr style=""font-size: 20px; color: #006193; font-weight: bold; font-family: 'Nunito', sans-serif;""><td colspan=2>Number of sessions per day</td></tr>
<tr class=""session-chart-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td style=""width:79%; text-align:center;""><img src=""{model.SessionsChart.ChartImagePath}""></td>
    <td style=""width:21%; text-align:left;"">{GetLegendHtml(model.SessionsChart.Legend)}</td>
</tr>
</table>
")}

{(model.StudentUsersChart.ChartImagePath == string.Empty ? ""
      : $@"
<table>
<tr style=""font-size: 20px; color: #006193; font-weight: bold; font-family: 'Nunito', sans-serif;""><td colspan=2>Student engagement</td></tr>
<tr class=""session-chart-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td style=""width:79%; text-align:center;""><img src=""{model.StudentUsersChart.ChartImagePath}""></td>
    <td style=""width:21%; text-align:left;"">{GetLegendHtml(model.StudentUsersChart.Legend)}</td>
</tr>
</table>
")}

<table>
<tr style=""font-size: 20px; color: #006193; font-weight: bold; font-family: 'Nunito', sans-serif;""><td colspan=2>Most popular activities (Top 5)</td></tr>
<tr class=""session-chart-row"" style=""display: flex; flex-direction: row;"">
    <td style=""width:70%; text-align:center;""><img src=""{model.PagesChart.ChartImagePath}""></td>
    <td style=""width:30%; text-align:left;"">{GetLegendHtml(model.PagesChart.Legend)}</td>
</tr>
</table>

{(model.TeacherUsersChart.ChartImagePath == string.Empty ? ""
      : $@"
<table>
<tr style=""font-size: 20px; color: #006193; font-weight: bold; font-family: 'Nunito', sans-serif;""><td colspan=2>Teacher engagement</td></tr>
<tr class=""session-chart-row"" style=""display: flex; flex-direction: row; margin-bottom: 10px;"">
    <td style=""width:79%; text-align:center;""><img src=""{model.TeacherUsersChart.ChartImagePath}""></td>
    <td style=""width:21%; text-align:left;"">{GetLegendHtml(model.TeacherUsersChart.Legend)}</td>
</tr>
</table>
")}

</div>

";

        private string GetLegendHtml(List<ChartLegendModel> legend)
        {
            var legendHtml = "";
            foreach (var entry in legend)
            {
                var color = $"rgba({entry.Color.R},{entry.Color.G},{entry.Color.B},{entry.Color.A})";
                legendHtml += $"<table><tr><td style=\"width:10px;\"><div style=\"width: 6px; height: 6px; border-radius: 50%; border: 2px solid {color}; background-color: {color}; margin-right: 10px;\"></div></td><td style=\"padding-right: 10px; color:{color}; text-align:left; font-weight: bold;\">{entry.Label}</td></tr></table>";
            }

            return legendHtml;
        }
    }
}