using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Fanex.DHWL.Business.Services.DailyChart
{
    public interface DailyChartService
    {
        string ExportDailyChart(Dictionary<int, double> sbFigure, Dictionary<int, double> totalFigure, ChartMtdFigureModel mtdFigure);
    }

    public class DailyChartServiceImp : DailyChartService
    {
        private readonly DateTimeServiceProvider dateTimeService;
        private readonly FormatService formatService;

        private readonly int million = 1000000;
        private readonly Log log;

        public DailyChartServiceImp(DateTimeServiceProvider dateTimeService, FormatService formatService, Log log)
        {
            this.dateTimeService = dateTimeService;
            this.formatService = formatService;
            this.log = log;
        }

        public string ExportDailyChart(Dictionary<int, double> sbFigure, Dictionary<int, double> totalFigure, ChartMtdFigureModel mtdFigure)
        {
            var font = new Font("Tahoma", 14);
            var fontMtd = new Font("Tahoma", 19);
            var sportBookChart = "Sportsbook";
            var otherProductsChart = "OtherProduct";
            var totalChart = "Total";

            Chart chart = new Chart();

            ConfigureChartAreas(totalFigure, font, chart);
            ConfigureChartLegends(fontMtd, chart, mtdFigure);
            ConfigurationSportBookChart(font, sportBookChart, chart);
            ConfigurationOtherProductsChart(font, otherProductsChart, chart);
            ConfigurationTotalChart(font, totalChart, chart);
            AssignDataToCharts(sbFigure, totalFigure, sportBookChart, otherProductsChart, totalChart, chart);

            return SaveChartToImage(chart);
        }

        private string SaveChartToImage(Chart chart)
        {
            var folder = $@"{AppConfigService.Config.Path}\{dateTimeService.DateTime.ToDate:yyyy}\{dateTimeService.DateTime.ToDate:MM}";
            Directory.CreateDirectory(folder);
            var fileName = $@"{folder}\House-Net-Income-Chart-On-{dateTimeService.DateTime.ToDate:yyyyMMdd}.png";
            chart.SaveImage(fileName, ImageFormat.Png);

            return fileName;
        }

        private void AssignDataToCharts(
            Dictionary<int, double> sbFigure,
            Dictionary<int, double> totalFigure,
            string sportBookChart,
            string otherProductsChart,
            string totalChart,
            Chart chart)
        {
            foreach (var month in sbFigure)
            {
                chart.Series[sportBookChart].Points.AddXY(month.Key, month.Value / million);
            }

            foreach (var month in totalFigure)
            {
                var otherValue = totalFigure[month.Key] - sbFigure[month.Key];
                chart.Series[otherProductsChart].Points.AddXY(month.Key, otherValue / million);

                var total = month.Value / million;
                var totalCutOffValue = Truncate(total, 1);
                chart.Series[totalChart].Points.AddXY(month.Key, totalCutOffValue);
            }
        }

        private double Truncate(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        private static void ConfigurationTotalChart(Font font, string totalChart, Chart chart)
        {
            chart.Series.Add(totalChart);
            chart.Series[totalChart].LegendText = "Total";
            chart.Series[totalChart].ChartType = SeriesChartType.Line;
            chart.Series[totalChart].Color = Color.FromArgb(0, 69, 134);
            chart.Series[totalChart].IsVisibleInLegend = true;
            chart.Series[totalChart].IsValueShownAsLabel = true;
            chart.Series[totalChart].LabelFormat = "#,##0.#m";
            chart.Series[totalChart].BorderWidth = 3;
            chart.Series[totalChart].Font = font;

            chart.Series[totalChart].MarkerStyle = MarkerStyle.Triangle;
            chart.Series[totalChart].MarkerSize = 15;
        }

        private static void ConfigurationOtherProductsChart(Font font, string otherProductsChart, Chart chart)
        {
            chart.Series.Add(otherProductsChart);
            chart.Series[otherProductsChart].LegendText = "Others";
            chart.Series[otherProductsChart].ChartType = SeriesChartType.Column;
            chart.Series[otherProductsChart].Color = Color.FromArgb(251, 180, 64);
            chart.Series[otherProductsChart].IsVisibleInLegend = true;
            chart.Series[otherProductsChart].IsValueShownAsLabel = false;
            chart.Series[otherProductsChart].LabelFormat = "#,##0.0m";
            chart.Series[otherProductsChart].BorderWidth = 1;
            chart.Series[otherProductsChart].Font = font;
        }

        private static void ConfigurationSportBookChart(Font font, string sportBookChart, Chart chart)
        {
            chart.Series.Add(sportBookChart);
            chart.Series[sportBookChart].LegendText = "SB";
            chart.Series[sportBookChart].ChartType = SeriesChartType.Column;
            chart.Series[sportBookChart].Color = Color.FromArgb(133, 184, 253);
            chart.Series[sportBookChart].IsVisibleInLegend = true;
            chart.Series[sportBookChart].IsValueShownAsLabel = false;
            chart.Series[sportBookChart].LabelFormat = "#,##0.0m";
            chart.Series[sportBookChart].BorderWidth = 1;
            chart.Series[sportBookChart].Font = font;
        }

        private void ConfigureChartLegends(Font font, Chart chart, ChartMtdFigureModel mtdFigure)
        {
            var fontMtd = new Font(font, FontStyle.Bold);
            var fromDate = dateTimeService.DateTime.FromDate;
            var title = $"DHWL [{fromDate:MMM-yyyy}]";
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
            chart.Titles[1].DockingOffset = -6;

            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].IsDockedInsideChartArea = false;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        private string MtdFigure(ChartMtdFigureModel mtdFigure)
        {
            var toDate = dateTimeService.DateTime.ToDate;
            var preToDate = new DateTimePreviousYearServiceImp(dateTimeService).ToDate;
            var mtd = $"MTD (as of {toDate:yyyy-MM-dd}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.Mtd))} [{mtdFigure.Margin * 100:#,##0.00}%]";
            var previousMtd = $"MTD (as of {preToDate:yyyy-MM-dd}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.PreviousMtd))} [{mtdFigure.PreviousMargin * 100:#,##0.00}%]";
            var diff = $"Diff %: {mtdFigure.DiffMtd} [{mtdFigure.DiffMargin}]";

            var strLog = new List<string>
            {
                $"Margin={mtdFigure.Margin}",
                $"PreviousMargin={mtdFigure.PreviousMargin}",
                $"DiffMargin={mtdFigure.DiffMargin}"
            };

            log.Log($"DHWL - {string.Join(",", strLog)}");

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
            var numberOfDays = totalFigure.Count;
            var width = (numberOfDays * 1920 / 31) + 50;
            width = Math.Max(width, 200);

            if (numberOfDays <= 9)
            {
                width = 650;
            }

            var maxValue = Math.Round(totalFigure.Max(s => s.Value) / million, 0) + 1;
            var minList = new List<double>();
            totalFigure.ToList().ForEach(m =>
            {
                minList.Add(totalFigure[m.Key]);
            });

            var minValue = minList.Min(s => s) / million - 1;
            minValue = Math.Round(Math.Min(minValue, 0), 0);

            chart.Size = new Size(width, 900);
            chart.ChartAreas.Add("ChartArea1");

            var positions = AppConfigService.Config.AreasPosition;
            var position = positions[numberOfDays];
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(position.InnerPlotPosition.X, position.InnerPlotPosition.Y, position.InnerPlotPosition.Width, position.InnerPlotPosition.Height);
            chart.ChartAreas[0].Position = new ElementPosition(position.Position.X, position.Position.Y, position.Position.Width, position.Position.Height);

            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0m";
            chart.ChartAreas[0].AxisY.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisY.Interval = 1;
            chart.ChartAreas[0].AxisY.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.ScaleView.Size = maxValue - minValue;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisY.Minimum = minValue;
            chart.ChartAreas[0].AxisY.Maximum = maxValue;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(217, 217, 217);

            chart.ChartAreas[0].AxisX.Title = "Date";
            chart.ChartAreas[0].AxisX.TitleFont = font;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisX.IsStartedFromZero = false;
            chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(217, 217, 217);
            chart.ChartAreas[0].AxisX.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            foreach (var day in totalFigure)
            {
                var labelX = new CustomLabel(day.Key - 1, day.Key + 1, day.Key.ToString(), 0, LabelMarkStyle.LineSideMark);
                if (IsWeekendDays(day.Key))
                {
                    labelX.ForeColor = Color.Red;
                }

                chart.ChartAreas[0].AxisX.CustomLabels.Add(labelX);
            }
        }

        private bool IsWeekendDays(int day)
        {
            var fromDate = dateTimeService.DateTime.FromDate;
            var currentMonth = fromDate.Month;
            var currentYear = fromDate.Year;
            var date = new DateTime(currentYear, currentMonth, day);

            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}