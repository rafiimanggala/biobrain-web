using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Biobrain.Application.Common.Models;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Reports.UsageReport;
using Biobrain.Application.Values;
using Biobrain.Domain.Constants;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

namespace Biobrain.Application.Services.Domain.Reports;

public class UsageReportChartService : IUsageReportChartService
{
    //private readonly string[] subjectColors = new[] { "#004876", "#0A552A", "#213F48", /*year10*/ "#00a1df", "#23A23E", "#277986", "#A20F3B", "#009490", "#9f73b2" };
    //private readonly string[] subjectColors = new[] { "#006193", "#0E602D", "#224851" };
    //private readonly string[] subjectColors = new[] { "#00a1df", "#23A23E", "#277986", /*year10*/ "#004876" };
    private readonly string[] pagesColors = new[] { "#004876", "#2296f3", "#8bc34a", "#ffbf00", "676767" };

    public UsageReportChartService(IDb db)
    {
    }

    // Dictionary<string, int> data
    public ChartModel GetPagesPieChartImagePath(string folderPath, Dictionary<string, int> pageViewsDictionary)
    {
        var fileName = $"{Guid.NewGuid()}.png";
        var filePath = Path.Combine(folderPath, fileName);

        var chart = new SKPieChart
                    {
                        Width = 220,
                        Height = 220,
                        Series = GetPieChartSeries(pageViewsDictionary),
                    };
        chart.SaveImage(filePath);
        return new ChartModel { ChartImagePath = filePath, Legend = chart.Series.Select(GetLegendByPieSeries).ToList() };
    }

    // Dictionary<string, int> data
    public ChartModel GetLinearChartImagePath(string folderPath, Dictionary<ReportKeyData, List<int>> dataDictionary, DateTime startDate, Constant.TimeInterval interval)
    {
        var fileName = $"{Guid.NewGuid()}.png";
        var filePath = Path.Combine(folderPath, fileName);

        var chart = new SKCartesianChart
                    {
                        Width = 700,
                        Height = 250,

                        Series = GetLineChartSeries(dataDictionary),
                    };

        var xAxes = chart.XAxes.First();
        xAxes.Labeler = d =>
                        {
                            var actualDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
                            actualDate = interval switch
                            {
                                Constant.TimeInterval.Day => actualDate.AddDays(d),
                                Constant.TimeInterval.Week => actualDate.AddDays(d * 7),
                                Constant.TimeInterval.Month => actualDate.AddMonths((int)d),
                                _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
                            };

                            return $"{actualDate:d MMM}";
                        };

        xAxes.LabelsRotation = -90;
        xAxes.Xo = 0;
        xAxes.Yo = 0;
        xAxes.MinLimit = 0;
        xAxes.MaxLimit = dataDictionary.Max(p => p.Value.Count) -1;
        xAxes.MinStep = 5;
        xAxes.ForceStepToMin = true;
        xAxes.ShowSeparatorLines = true;

        var yAxes = chart.YAxes.First();
        yAxes.Xo = 0;
        yAxes.Yo = 0;
        yAxes.MinLimit = 0;
        yAxes.MinStep = 1;
        yAxes.ShowSeparatorLines = true;

        chart.SaveImage(filePath);
        return new ChartModel
               {
                   ChartImagePath = filePath,
                   Legend = chart.Series.Select(x => new ChartLegendModel { Color = GetLineSeriesColor(x), Label = $"{x.Name}" }).ToList()
               };
    }

    private ChartLegendModel GetLegendByPieSeries(ISeries series)
    {
        var enumirator = series.Values.GetEnumerator();
        enumirator.Reset();
        enumirator.MoveNext();
        return new ChartLegendModel
               { Color = GetPieSeriesColor(series), Label = $"{PageLabelMapper.GetLabel(series.Name)} ({enumirator.Current})" };
    }

    private IEnumerable<ISeries> GetPieChartSeries(Dictionary<string, int> data)
    {
        var series = new List<ISeries>();
        var i = 0;
        foreach (var entry in data.OrderByDescending(_ => _.Value))
        {
            series.Add(new PieSeries<int>
                       {
                           Name = entry.Key, Values = [entry.Value], DataLabelsPosition = PolarLabelsPosition.Middle,
                           Fill = new SolidColorPaint(SKColor.Parse(pagesColors[i]))
                       });
            i++;
            // Звпускаем цвета с начала
            if (i > 4)
                i = 0;
        }

        return series;
    }

    private static Color GetPieSeriesColor(ISeries series)
    {
        PieSeries<int> pieSeries = series as PieSeries<int>;

        return pieSeries?.Fill is SolidColorPaint fill
                   ? Color.FromArgb(fill.Color.Alpha, fill.Color.Red, fill.Color.Green, fill.Color.Blue)
                   : Color.White;
    }

    private IEnumerable<ISeries> GetLineChartSeries(Dictionary<ReportKeyData, List<int>> data)
    {
        var series = new List<ISeries>();
        foreach (var entry in data)
        {
            var color = SKColor.Parse(entry.Key.Color);
            var fillColor = new SKColor(color.Red, color.Green, color.Blue, 0);
            series.Add(new LineSeries<int>
                       {
                           Name = entry.Key.Name,
                           Values = entry.Value,
                           LineSmoothness = 0,
                           Stroke = new SolidColorPaint(color, 2),
                           Fill = new SolidColorPaint(fillColor),
                           GeometryStroke = new SolidColorPaint(color, 3),
                           GeometryFill = new SolidColorPaint(color),
                           GeometrySize = 3,
                       });
        }

        return series;
    }

    private static Color GetLineSeriesColor(ISeries series)
    {
        LineSeries<int> pieSeries = series as LineSeries<int>;

        return pieSeries?.Stroke is SolidColorPaint fill
                   ? Color.FromArgb(fill.Color.Alpha, fill.Color.Red, fill.Color.Green, fill.Color.Blue)
                   : Color.White;
    }
}