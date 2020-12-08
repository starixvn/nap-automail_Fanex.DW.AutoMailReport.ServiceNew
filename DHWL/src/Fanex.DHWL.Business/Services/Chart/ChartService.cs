using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Fanex.DHWL.Business.Services
{
    public interface ChartService
    {
        string ExportDailyChart(Dictionary<int, double> sbfigure, Dictionary<int, double> totalfigure, Dictionary<int, double> cn88figure);
    }

    public class ChartServiceImp : ChartService
    {
        private readonly DateTimeServiceProvider dateTimeService;

        private readonly int milions = 1000000;

        public ChartServiceImp(DateTimeServiceProvider dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public string ExportDailyChart(Dictionary<int, double> sbfigure, Dictionary<int, double> totalfigure, Dictionary<int, double> cn88figure)
        {
            var font = new Font("Tahoma", 14);
            var sportbookChart = "Sportsbook";
            var otherProductsChart = "OtherProduct";
            var totalChart = "Total";
            var totalChartWithoutCN88 = "TotalWithoutCN88";

            Chart chart = new Chart();

            ConfigurateChartAreas(totalfigure, cn88figure, font, chart);
            ConfigurateChartLegends(font, chart);
            ConfigurationSportbookChart(font, sportbookChart, chart);
            ConfigurationOtherProductsChart(font, otherProductsChart, chart);
            ConfiguationTotalChart(font, totalChart, chart);
            ConfiguationTotalWithoutCN88Chart(font, totalChartWithoutCN88, chart);
            AssignDataToCharts(sbfigure, totalfigure, cn88figure, sportbookChart, otherProductsChart, totalChart, totalChartWithoutCN88, chart);

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
            Dictionary<int, double> sbfigure,
            Dictionary<int, double> totalFigure,
            Dictionary<int, double> cn88figure,
            string sportbookChart,
            string otherProductsChart,
            string totalChart,
            string totalChartWithoutCN88,
            Chart chart)
        {
            foreach (var month in sbfigure)
            {
                chart.Series[sportbookChart].Points.AddXY(month.Key, month.Value / milions);
            }

            foreach (var month in totalFigure)
            {
                var otherValue = totalFigure[month.Key] - sbfigure[month.Key];
                chart.Series[otherProductsChart].Points.AddXY(month.Key, otherValue / milions);

                var total = month.Value / milions;
                var totalCutOffValue = Truncate(total, 1);
                chart.Series[totalChart].Points.AddXY(month.Key, totalCutOffValue);

                var totalWithoutCN88 = (totalFigure[month.Key] - cn88figure[month.Key]) / milions;
                var totalWithoutCN88CutOffValue = Truncate(totalWithoutCN88, 1);
                chart.Series[totalChartWithoutCN88].Points.AddXY(month.Key, totalWithoutCN88CutOffValue);
            }
        }

        private double Truncate(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        private static void ConfiguationTotalChart(Font font, string totalChart, Chart chart)
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

        private static void ConfiguationTotalWithoutCN88Chart(Font font, string totalWithoutCN88Chart, Chart chart)
        {
            chart.Series.Add(totalWithoutCN88Chart);
            chart.Series[totalWithoutCN88Chart].LegendText = "Total (w/o C8)";
            chart.Series[totalWithoutCN88Chart].ChartType = SeriesChartType.Line;
            chart.Series[totalWithoutCN88Chart].Color = Color.FromArgb(255, 66, 14);
            chart.Series[totalWithoutCN88Chart].IsVisibleInLegend = true;
            chart.Series[totalWithoutCN88Chart].IsValueShownAsLabel = true;
            chart.Series[totalWithoutCN88Chart].LabelFormat = "#,##0.#m";
            chart.Series[totalWithoutCN88Chart].LabelForeColor = Color.FromArgb(138, 27, 27);
            chart.Series[totalWithoutCN88Chart].BorderWidth = 3;
            chart.Series[totalWithoutCN88Chart].Font = font;

            chart.Series[totalWithoutCN88Chart].MarkerStyle = MarkerStyle.Diamond;
            chart.Series[totalWithoutCN88Chart].MarkerSize = 15;
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

        private static void ConfigurationSportbookChart(Font font, string sportbookChart, Chart chart)
        {
            chart.Series.Add(sportbookChart);
            chart.Series[sportbookChart].LegendText = "SB";
            chart.Series[sportbookChart].ChartType = SeriesChartType.Column;
            chart.Series[sportbookChart].Color = Color.FromArgb(133, 184, 253);
            chart.Series[sportbookChart].IsVisibleInLegend = true;
            chart.Series[sportbookChart].IsValueShownAsLabel = false;
            chart.Series[sportbookChart].LabelFormat = "#,##0.0m";
            chart.Series[sportbookChart].BorderWidth = 1;
            chart.Series[sportbookChart].Font = font;
        }

        private void ConfigurateChartLegends(Font font, Chart chart)
        {
            var fromDate = dateTimeService.DateTime.FromDate;
            var title = $"DHWL [{fromDate:MMM-yyyy}]";
            chart.Legends.Add("legend1");
            chart.Titles.Add("Title");
            chart.Titles[0].Text = title;
            chart.Titles[0].Font = font;
            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].IsDockedInsideChartArea = false;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        private void ConfigurateChartAreas(Dictionary<int, double> totalFigure, Dictionary<int, double> cn88figure, Font font, Chart chart)
        {
            var numberofDays = totalFigure.Count;
            var width = (numberofDays * 1920 / 31) + 50;
            width = Math.Max(width, 200);

            var maxValue = Math.Round(totalFigure.Max(s => s.Value) / milions, 0) + 1;
            var minList = new List<double>();
            totalFigure.ToList().ForEach(m =>
            {
                minList.Add(totalFigure[m.Key] - cn88figure[m.Key]);
            });

            var minValue = minList.Min(s => s) / milions - 1;
            minValue = Math.Round(Math.Min(minValue, 0), 0);

            chart.Size = new Size(width, 900);
            chart.ChartAreas.Add("ChartArea1");

            var positions = AppConfigService.Config.AreasPosition;
            var position = positions[numberofDays];
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
            var currentyear = fromDate.Year;
            var date = new DateTime(currentyear, currentMonth, day);

            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}