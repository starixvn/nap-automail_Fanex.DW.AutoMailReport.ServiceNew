using Fanex.DHWL.Business.Models;
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
        string ExportMonthlyChart(Dictionary<int, double> credit, Dictionary<int, double> lic, Dictionary<int, double> total, ChartMtdFigureModel mtdFigure);
    }

    public class MonthlyChartServiceImp : MonthlyChartService
    {
        private readonly DateTimeServiceProvider dateTimeService;
        private readonly FormatService formatService;

        private readonly int million = 1000000;

        public MonthlyChartServiceImp(DateTimeServiceProvider dateTimeService, FormatService formatService)
        {
            this.dateTimeService = dateTimeService;
            this.formatService = formatService;
        }

        public string ExportMonthlyChart(Dictionary<int, double> credit, Dictionary<int, double> lic, Dictionary<int, double> total, ChartMtdFigureModel mtdFigure)
        {
            var font = new Font("Tahoma", 14);
            var fontMtd = new Font("Tahoma", 19);
            var creditChart = "Credit";
            var licChart = "Lic";

            Chart chart = new Chart();

            ConfigureChartAreas(total, font, chart);
            ConfigureChartLegends(fontMtd, chart, mtdFigure);
            ConfigurationCreditChart(font, creditChart, chart);
            ConfigurationLicChart(font, licChart, chart);

            AssignDataToCharts(credit, lic, creditChart, licChart, chart);

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
            Dictionary<int, double> credit,
            Dictionary<int, double> lic,
            string creditChart,
            string licChart,
            Chart chart)
        {
            foreach (var month in credit)
            {
                chart.Series[creditChart].Points.AddXY(month.Key, month.Value / million);
            }

            foreach (var month in lic)
            {
                chart.Series[licChart].Points.AddXY(month.Key, month.Value / million);
            }
        }

        private static void ConfigurationLicChart(Font font, string licChart, Chart chart)
        {
            chart.Series.Add(licChart);
            chart.Series[licChart].LegendText = "Lic";
            chart.Series[licChart].ChartType = SeriesChartType.StackedColumn;
            chart.Series[licChart].Color = Color.FromArgb(251, 180, 64);
            chart.Series[licChart].IsVisibleInLegend = true;
            chart.Series[licChart].IsValueShownAsLabel = true;
            chart.Series[licChart].LabelFormat = "#,##0.0m";
            chart.Series[licChart].BorderWidth = 1;
            chart.Series[licChart].Font = font;
        }

        private static void ConfigurationCreditChart(Font font, string creditChart, Chart chart)
        {
            chart.Series.Add(creditChart);
            chart.Series[creditChart].LegendText = "Credit";
            chart.Series[creditChart].ChartType = SeriesChartType.StackedColumn;
            chart.Series[creditChart].Color = Color.FromArgb(133, 184, 253);
            chart.Series[creditChart].IsVisibleInLegend = true;
            chart.Series[creditChart].IsValueShownAsLabel = true;
            chart.Series[creditChart].LabelFormat = "#,##0.0m";
            chart.Series[creditChart].BorderWidth = 1;
            chart.Series[creditChart].Font = font;
        }

        private void ConfigureChartLegends(Font font, Chart chart, ChartMtdFigureModel mtdFigure)
        {
            var fontMtd = new Font(font, FontStyle.Bold);
            var fromDate = dateTimeService.DateTime.FromDate;
            var title = $"DHWL YTD Chart [{fromDate:MMM-yyyy}]";
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
            chart.Legends[0].BackColor = Color.FromArgb(231, 240, 249);
            chart.Legends[0].Alignment = StringAlignment.Center;
        }

        private string MtdFigure(ChartMtdFigureModel mtdFigure)
        {
            var toDate = dateTimeService.DateTime.ToDate;
            var preToDate = new DateTimePreviousYearServiceImp(dateTimeService).ToDate;
            var mtd = $"MTD (as of {toDate:yyyy-MM-dd}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.Mtd))} [{mtdFigure.Margin:P}]";
            var previousMtd = $"MTD (as of {preToDate:yyyy-MM-dd}): {formatService.CutNumberOff(Convert.ToDecimal(mtdFigure.PreviousMtd))} [{mtdFigure.PreviousMargin:P}]";
            var diff = $"Diff %: {mtdFigure.DiffMtd} [{mtdFigure.DiffMargin}]";

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
                width = 700;
            }

            var maxValue = Math.Round(totalFigure.Max(s => s.Value) / million, 0) + 20;
            var minList = new List<double>();
            totalFigure.ToList().ForEach(m =>
            {
                minList.Add(totalFigure[m.Key]);
            });

            var minValue = minList.Min(s => s) / million - 20;
            minValue = Math.Round(Math.Min(minValue, 0), 0);

            chart.Size = new Size(width, 900);
            chart.ChartAreas.Add("ChartArea1");

            var positions = AppConfigService.Config.AreasPosition;
            var position = positions[numberOfDays];
            chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(position.InnerPlotPosition.X, position.InnerPlotPosition.Y, position.InnerPlotPosition.Width, position.InnerPlotPosition.Height);
            chart.ChartAreas[0].Position = new ElementPosition(position.Position.X, position.Position.Y, position.Position.Width, position.Position.Height);
            chart.ChartAreas[0].BackColor = Color.FromArgb(231, 240, 249);

            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "#,##0m";
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
            chart.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(217, 217, 217);
            chart.ChartAreas[0].AxisX.MinorTickMark.TickMarkStyle = TickMarkStyle.InsideArea;

            foreach (var day in totalFigure)
            {
                var labelX = new CustomLabel(day.Key - 1, day.Key + 1, day.Key.ToString(), 0, LabelMarkStyle.LineSideMark);
                chart.ChartAreas[0].AxisX.CustomLabels.Add(labelX);
            }
        }
    }
}