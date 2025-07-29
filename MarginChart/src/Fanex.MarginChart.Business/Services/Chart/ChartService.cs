using Fanex.MarginChart.Business.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Fanex.MarginChart.Business.Services
{
    public interface ChartService
    {
        string ExportDailyChart(ProductGroup productGroup, string type, Dictionary<int, double> winloss,
            Dictionary<int, double> turnover);
    }

    public class ChartServiceImp : ChartService
    {
        private readonly DateTimeServiceProvider dateRangeServiceProfier;
        private readonly FormatService formatService;
        private readonly Color marginColorTrendLineChart = Color.FromArgb(247, 154, 78);
        private readonly Color marginColorLabelChart = Color.FromArgb(0, 0, 0);
        private readonly Color turnoverColorColumnChart = Color.FromArgb(91, 155, 213);
        private readonly Color gridLineColor = Color.FromArgb(242, 242, 242);

        private int convertValue = 1000000;
        private int axisXIntervalTurnover = 100;
        private double axisY2IntervalMargin = 0.01;

        public ChartServiceImp(DateTimeServiceProvider dateRangeServiceProfier, FormatService formatService)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            this.formatService = formatService;
        }

        public string ExportDailyChart(ProductGroup productGroup, string type, Dictionary<int, double> winloss,
            Dictionary<int, double> turnover)
        {
            var font = new Font("Tahoma", 14);
            var turnoverChart = "Turnover";
            var marginChart = "Margin";

            convertValue = productGroup.ConvertValue;
            axisXIntervalTurnover = productGroup.AxisXIntervalTurnover;
            axisY2IntervalMargin = productGroup.AxisY2IntervalMargin;

            Chart chart = new Chart();

            ConfigurateChartAreasTurnover(turnover, font, chart);
            ConfigurationChartAreasMargin(winloss, turnover, font, chart);
            ConfigurateChartLegends(productGroup.HeaderChart, type, font, chart);
            ConfigurationTurnoverChart(font, turnoverChart, chart);
            ConfiguationMarginChart(font, marginChart, chart);
            AssignDataToCharts(winloss, turnover, turnoverChart, marginChart, chart);

            return SaveChartToImage(productGroup.ProductCategory, productGroup.Subject, type, chart);
        }

        private string SaveChartToImage(string category, string subject, string type, Chart chart)
        {
            var dateService = dateRangeServiceProfier.DateTime;
            var date = dateService.ToDate;
            var fileName = $@"{subject}-{type}-{dateService.FromDate:yyyyMMdd}-to-{dateService.ToDate:yyyyMMdd}.png";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}\{category}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";

            try
            {
                chart.SaveImage(path, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                path = string.Empty;
            }

            return path;
        }

        private void AssignDataToCharts(Dictionary<int, double> winlost, Dictionary<int, double> turnover,
            string turnoverChart, string marginChart, Chart chart)
        {
            foreach (var month in turnover)
            {
                var turnoverCutOffValue = formatService.Truncate(turnover[month.Key] / convertValue, 1);
                chart.Series[turnoverChart].Points.AddXY(month.Key, turnoverCutOffValue);

                var magin = Math.Abs(turnover[month.Key]) < 0 ? 0 : winlost[month.Key] / turnover[month.Key];
                chart.Series[marginChart].Points.AddXY(month.Key, magin);
                chart.Series[marginChart].YAxisType = AxisType.Secondary;
            }
        }

        private void ConfiguationMarginChart(Font font, string marginChart, Chart chart)
        {
            chart.Series.Add(marginChart);
            chart.Series[marginChart].LegendText = "Margin";
            chart.Series[marginChart].ChartType = SeriesChartType.Line;
            chart.Series[marginChart].Color = marginColorTrendLineChart;
            chart.Series[marginChart].IsVisibleInLegend = true;
            chart.Series[marginChart].IsValueShownAsLabel = true;
            chart.Series[marginChart].LabelFormat = "##0.#0%";
            chart.Series[marginChart].BorderWidth = 3;
            chart.Series[marginChart].Font = font;
            chart.Series[marginChart].LabelForeColor = marginColorLabelChart;
        }

        private void ConfigurationTurnoverChart(Font font, string turnoverChart, Chart chart)
        {
            chart.Series.Add(turnoverChart);
            chart.Series[turnoverChart].LegendText = "House Turnover";
            chart.Series[turnoverChart].ChartType = SeriesChartType.Column;
            chart.Series[turnoverChart].Color = turnoverColorColumnChart;
            chart.Series[turnoverChart].IsVisibleInLegend = true;
            chart.Series[turnoverChart].IsValueShownAsLabel = false;
            chart.Series[turnoverChart].LabelFormat = "#,##0.0k";
            chart.Series[turnoverChart].BorderWidth = 1;
            chart.Series[turnoverChart].Font = font;
        }

        private void ConfigurateChartLegends(string headerChart, string type, Font font, Chart chart)
        {
            var fromDate = dateRangeServiceProfier.DateTime.FromDate;
            var title = $"{headerChart} - {type} [{fromDate:MMM-yyyy}]";
            chart.Legends.Add("legend1");
            chart.Titles.Add("Title");
            chart.Titles[0].Text = title;
            chart.Titles[0].Font = font;
            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].IsDockedInsideChartArea = false;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        private void ConfigurateChartAreasTurnover(Dictionary<int, double> turnover, Font font, Chart chart)
        {
            var numberofDays = turnover.Count;
            var width = (numberofDays * 1500 / 31) + 50;
            width = Math.Max(width, 200);

            var maxValue = Math.Round(turnover.Max(s => s.Value) / convertValue, 0) + axisXIntervalTurnover;
            var minValue = 0;

            chart.Size = new Size(width, 900);
            chart.ChartAreas.Add("ChartArea");

            var positions = AppConfigService.Config.AreasPosition;
            var position = positions[numberofDays];
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(position.InnerPlotPosition.X,
                position.InnerPlotPosition.Y, position.InnerPlotPosition.Width, position.InnerPlotPosition.Height);
            chart.ChartAreas[0].Position = new ElementPosition(position.Position.X, position.Position.Y,
                position.Position.Width, position.Position.Height);

            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0k";
            chart.ChartAreas[0].AxisY.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = turnoverColorColumnChart;
            chart.ChartAreas[0].AxisY.Interval = axisXIntervalTurnover;
            chart.ChartAreas[0].AxisY.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.ScaleView.Size = maxValue - minValue;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisY.Minimum = minValue;
            chart.ChartAreas[0].AxisY.Maximum = maxValue;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = gridLineColor;
            chart.ChartAreas[0].AxisY.LineColor = turnoverColorColumnChart;

            chart.ChartAreas[0].AxisX.Title = "Date";
            chart.ChartAreas[0].AxisX.TitleFont = font;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisX.Interval = 1;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = gridLineColor;
            chart.ChartAreas[0].AxisX.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            var count = 1;
            foreach (var day in turnover)
            {
                var labelX = new CustomLabel(day.Key - 1, day.Key + 1, day.Key.ToString(), 0,
                    LabelMarkStyle.LineSideMark);
                if (IsWeekendDays(day.Key))
                {
                    labelX.ForeColor = Color.Red;
                }

                chart.ChartAreas[0].AxisX.CustomLabels.Add(labelX);

                if (count >= numberofDays)
                {
                    break;
                }

                count++;
            }
        }

        private void ConfigurationChartAreasMargin(Dictionary<int, double> winloss, Dictionary<int, double> turnover,
            Font font, Chart chart)
        {
            var margins = new List<double>();
            winloss.ToList().ForEach(w =>
            {
                var wl = w.Value;
                var to = turnover[w.Key];
                var margin = Math.Abs(to) <= 0 ? 0 : wl / to;
                margins.Add(margin);
            });

            var maxValue = Math.Round(margins.Max() + 0.005, 2);
            var minValue = Math.Round(margins.Min() - 0.005, 2);
            minValue = Math.Min(minValue, 0);

            chart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart.ChartAreas[0].AxisY2.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY2.LabelStyle.Format = "##0%";
            chart.ChartAreas[0].AxisY2.LabelStyle.Font = font;
            chart.ChartAreas[0].AxisY2.LabelStyle.ForeColor = marginColorTrendLineChart;
            chart.ChartAreas[0].AxisY2.Interval = axisY2IntervalMargin;
            chart.ChartAreas[0].AxisY2.ScaleView.SizeType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY2.ScaleView.Size = maxValue - minValue;
            chart.ChartAreas[0].AxisY2.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisY2.Minimum = minValue;
            chart.ChartAreas[0].AxisY2.Maximum = maxValue;
            chart.ChartAreas[0].AxisY2.MajorGrid.LineColor = gridLineColor;
            chart.ChartAreas[0].AxisY2.LineColor = marginColorTrendLineChart;
        }

        private bool IsWeekendDays(int day)
        {
            var fromDate = dateRangeServiceProfier.DateTime.FromDate;
            var currentMonth = fromDate.Month;
            var currentyear = fromDate.Year;
            var date = new DateTime(currentyear, currentMonth, day);

            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}