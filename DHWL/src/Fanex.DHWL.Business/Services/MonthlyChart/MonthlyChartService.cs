using Fanex.DHWL.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Fanex.DHWL.Business.Services.MonthlyChart
{
    public interface MonthlyChartService
    {
        string ExportMonthlyChart(Dictionary<int, double> lastYear, Dictionary<int, double> currentYear,
            ChartMtdFigureModel mtdFigure, string depositType);
    }

    public class MonthlyChartServiceImp : MonthlyChartService
    {
        private readonly DateTimeServiceProvider dateTimeService;
        private readonly FormatService formatService;
        private readonly Log log;

        private const int Million = 1000;

        public MonthlyChartServiceImp(DateTimeServiceProvider dateTimeService, FormatService formatService, Log log)
        {
            this.dateTimeService = dateTimeService;
            this.formatService = formatService;
            this.log = log;
        }

        public string ExportMonthlyChart(Dictionary<int, double> lastYear, Dictionary<int, double> currentYear,
            ChartMtdFigureModel mtdFigure, string depositType)
        {
            var font = new Font("Tahoma", 14);
            var fontMtd = new Font("Tahoma", 19);

            var lastYearChart = "Last_Year_Chart";
            var currentYearChart = "Current_Year_Chart";
            var date = dateTimeService.DateTime.ToDate;
            var legendTextLastYear = $"{date.Year - 1} {depositType}";
            var legendTextCurrentYear = $"{date.Year} {depositType}";

            var chart = new Chart();

            var maxLastYear = lastYear.Max(s => s.Value);
            var maxCurrentYear = currentYear.Max(s => s.Value);
            var data = maxLastYear > maxCurrentYear ? lastYear : currentYear;

            LogFile(lastYear, currentYear, mtdFigure, depositType, date);

            ConfigureChartAreas(data, font, chart);
            ConfigureChartLegends(fontMtd, chart, mtdFigure, depositType);

            ConfigurationLastYearChart(font, lastYearChart, legendTextLastYear, chart);
            ConfigurationCurrentYearChart(font, currentYearChart, legendTextCurrentYear, chart);

            AssignDataToCharts(lastYear, currentYear, lastYearChart, currentYearChart, chart);

            return SaveChartToImage(chart, depositType);
        }

        private static void LogFile(Dictionary<int, double> lastYear, Dictionary<int, double> currentYear,
            ChartMtdFigureModel mtdFigure,
            string depositType, DateTime date)
        {
            var jsonLastYear = JsonConvert.SerializeObject(lastYear, Formatting.Indented);
            var jsonCurrentYear = JsonConvert.SerializeObject(currentYear, Formatting.Indented);
            var jsonMtdFigure = JsonConvert.SerializeObject(mtdFigure, Formatting.Indented);
            var path = $@"{AppConfigService.Config.Path}/{date:yyyyMMdd}";
            Directory.CreateDirectory(path);
            File.WriteAllText($@"{path}/DHWL-YTD-LastYear-{depositType}-{date:yyyy-MM}.json", jsonLastYear);
            File.WriteAllText($@"{path}/DHWL-YTD-CurrentYear-{depositType}-{date:yyyy-MM}.json", jsonCurrentYear);
            File.WriteAllText($@"{path}/DHWL-YTD-MtdFigure-{depositType}-{date:yyyy-MM}.json", jsonMtdFigure);
        }

        private string SaveChartToImage(Chart chart, string depositType)
        {
            var folder =
                $@"{AppConfigService.Config.Path}\{dateTimeService.DateTime.ToDate:yyyy}\{dateTimeService.DateTime.ToDate:MM}";
            Directory.CreateDirectory(folder);
            var fileName = $@"{folder}\DHWL-YTD-Chart-{depositType}-{dateTimeService.DateTime.ToDate:yyyy-MM}.png";
            chart.SaveImage(fileName, ImageFormat.Png);

            return fileName;
        }

        private void AssignDataToCharts(
            Dictionary<int, double> lastYear,
            Dictionary<int, double> currentYear,
            string lastYearChart,
            string currentYearChart,
            Chart chart)
        {
            foreach (var month in lastYear)
            {
                chart.Series[lastYearChart].Points.AddXY(month.Key, month.Value / Million);
            }

            foreach (var month in currentYear)
            {
                chart.Series[currentYearChart].Points.AddXY(month.Key, month.Value / Million);
            }
        }

        private static void ConfigurationLastYearChart(Font font, string lastYearChart, string legendText, Chart chart)
        {
            chart.Series.Add(lastYearChart);
            chart.Series[lastYearChart].LegendText = legendText;
            chart.Series[lastYearChart].ChartType = SeriesChartType.Column;
            chart.Series[lastYearChart].Color = Color.FromArgb(133, 184, 253);
            chart.Series[lastYearChart].IsVisibleInLegend = true;
            chart.Series[lastYearChart].IsValueShownAsLabel = true;
            chart.Series[lastYearChart].LabelFormat = "#,##0.0k";
            chart.Series[lastYearChart].BorderWidth = 1;
            chart.Series[lastYearChart].Font = font;
        }

        private static void ConfigurationCurrentYearChart(Font font, string currentYearChart, string legendText,
            Chart chart)
        {
            chart.Series.Add(currentYearChart);
            chart.Series[currentYearChart].LegendText = legendText;
            chart.Series[currentYearChart].ChartType = SeriesChartType.Column;
            chart.Series[currentYearChart].Color = Color.FromArgb(251, 180, 64);
            chart.Series[currentYearChart].IsVisibleInLegend = true;
            chart.Series[currentYearChart].IsValueShownAsLabel = true;
            chart.Series[currentYearChart].LabelFormat = "#,##0.0k";
            chart.Series[currentYearChart].BorderWidth = 1;
            chart.Series[currentYearChart].Font = font;
        }

        private void ConfigureChartLegends(Font font, Chart chart, ChartMtdFigureModel mtdFigure, string depositType)
        {
            var fontMtd = new Font(font, FontStyle.Bold);
            var fromDate = dateTimeService.DateTime.FromDate;

            var title = depositType.Equals("Total")
                ? $"DHWL YTD [{fromDate:MMM-yyyy}]"
                : $"{depositType.ToUpper()} DHWL YTD [{fromDate:MMM-yyyy}]";
            chart.Legends.Add("legend1");
            chart.Titles.Add("Title");
            chart.Titles[0].Text = title;
            chart.Titles[0].Font = fontMtd;
            chart.Titles[0].Alignment = ContentAlignment.TopCenter;
            chart.Titles[0].DockingOffset = -3;

            chart.Titles.Add("MTD");
            chart.Titles[1].Text = MtdFigure(mtdFigure);
            chart.Titles[1].Font = fontMtd;
            chart.Titles[1].Alignment = ContentAlignment.TopLeft;
            chart.Titles[1].ForeColor = Color.FromArgb(44, 185, 91);
            chart.Titles[1].DockingOffset = -3;

            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].IsDockedInsideChartArea = false;
            chart.Legends[0].BackColor = Color.FromArgb(231, 240, 249);
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        private string MtdFigure(ChartMtdFigureModel mtdFigure)
        {
            var toDate = dateTimeService.DateTime.ToDate;
            var preToDate = new DateTimePreviousYearServiceImp(dateTimeService).ToDate;
            var mtd =
                $"YTD (as of {toDate:MMM yyyy}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.Mtd))} [{mtdFigure.Margin * 100:F}%]";
            var previousMtd =
                $"YTD (as of {preToDate:MMM yyyy}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.PreviousMtd))} [{mtdFigure.PreviousMargin * 100:F}%]";
            var diff = $"Diff %: {mtdFigure.DiffMtd} [{mtdFigure.DiffMargin}]";

            var strLog = new List<string>
            {
                $"Margin={mtdFigure.Margin}",
                $"PreviousMargin={mtdFigure.PreviousMargin}",
                $"DiffMargin={mtdFigure.DiffMargin}"
            };

            log.Log($"DHWL YTD - {string.Join(",", strLog)}");

            var result = new List<string>
            {
                mtd,
                previousMtd,
                diff
            };

            return string.Join(Environment.NewLine, result);
        }

        private void ConfigureChartAreas(Dictionary<int, double> totalFigure, Font font, Chart chart)
        {
            var numberOfDays = totalFigure.Count * 2;
            var width = (numberOfDays * 1920 / 31) + 50;
            width = Math.Max(width, 200);

            if (numberOfDays <= 9)
            {
                width = 650;
            }

            var maxValue = Math.Round(totalFigure.Max(s => s.Value) / Million, 0) + 40;
            var minList = new List<double>();
            totalFigure.ToList().ForEach(m => { minList.Add(totalFigure[m.Key]); });

            var minValue = minList.Min(s => s) / Million;
            minValue = Math.Round(Math.Min(minValue, 0), 0);

            chart.Size = new Size(width, 900);
            chart.ChartAreas.Add("ChartArea1");

            var positions = AppConfigService.Config.AreasPosition;
            var position = positions[numberOfDays];
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(position.InnerPlotPosition.X,
                position.InnerPlotPosition.Y, position.InnerPlotPosition.Width, position.InnerPlotPosition.Height);
            chart.ChartAreas[0].Position = new ElementPosition(position.Position.X, position.Position.Y,
                position.Position.Width, position.Position.Height);
            chart.ChartAreas[0].BackColor = Color.FromArgb(231, 240, 249);

            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0k";
            chart.ChartAreas[0].AxisY.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisY.Interval = 20;
            chart.ChartAreas[0].AxisY.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.ScaleView.Size = maxValue - minValue;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisY.Minimum = minValue;
            chart.ChartAreas[0].AxisY.Maximum = maxValue;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(217, 217, 217);

            chart.ChartAreas[0].AxisX.TitleFont = font;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisX.IsStartedFromZero = false;
            chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(217, 217, 217);
            chart.ChartAreas[0].AxisX.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            var maxItem = totalFigure.Count;
            var monthYear = Month();
            var toMonth = dateTimeService.DateTime.ToDate.Month;
            for (var month = 1; month <= maxItem; month++)
            {
                var labelX = new CustomLabel(month - 1, month + 1, monthYear[month], 0, LabelMarkStyle.LineSideMark);
                if (month > toMonth)
                {
                    labelX.ForeColor = Color.Transparent;
                }

                chart.ChartAreas[0].AxisX.CustomLabels.Add(labelX);
            }
        }

        private static Dictionary<int, string> Month()
        {
            var months = new Dictionary<int, string>();
            for (var month = 1; month <= 12; month++)
            {
                var today = DateTime.Today;
                var date = new DateTime(today.Year, month, 01);
                months.Add(month, $"{date:MMM}");
            }

            return months;
        }
    }
}