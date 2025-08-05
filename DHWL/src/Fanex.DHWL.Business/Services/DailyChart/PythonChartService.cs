using Fanex.DHWL.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.DHWL.Business.Services.DailyChart
{
    public class DHWLChartRequest
    {
        public Dictionary<string, double> SbData { get; set; }

        public Dictionary<string, double> TotalData { get; set; }

        public double Mtd { get; set; }

        public double PreviousMtd { get; set; }

        public string DiffMtd { get; set; }

        public double Margin { get; set; }

        public double PreviousMargin { get; set; }

        public string DiffMargin { get; set; }

        public string Title { get; set; }

        public string MtdDate { get; set; }

        public string PreviousMtdDate { get; set; }
    }

    public class PythonChartService
    {
        private const string ApiUrl = "http://pythonchartapi.starix-klprod.gamora.prod/dhwl";
        private static readonly DateTimeServiceProvider dateTimeService = new DateTimeServiceProviderImp();

        private static readonly DateTimeService dateTimePreviousYearService =
            new DateTimePreviousYearServiceImp(dateTimeService);

        private static readonly HttpClient client = new HttpClient();
        private static readonly Log log = new LogImp();

        public async Task GenerateChart(
            Dictionary<int, double> sbFigure,
            Dictionary<int, double> totalFigure,
            ChartMtdFigureModel mtdFigure,
            string title,
            string chartName)
        {
            var toDate = dateTimeService.DateTime.ToDate;
            var preToDate = dateTimePreviousYearService.ToDate;
            var fromDate = dateTimeService.DateTime.FromDate;
            var request = new DHWLChartRequest
            {
                SbData = sbFigure.ToDictionary(k => k.Key.ToString(), v => v.Value),
                TotalData = totalFigure.ToDictionary(k => k.Key.ToString(), v => v.Value),
                Mtd = mtdFigure.Mtd,
                PreviousMtd = mtdFigure.PreviousMtd,
                DiffMtd = mtdFigure.DiffMtd,
                Margin = mtdFigure.Margin * 100,
                PreviousMargin = mtdFigure.PreviousMargin * 100,
                DiffMargin = mtdFigure.DiffMargin,
                Title = $"{title} [{fromDate:MMM-yyyy}]",
                MtdDate = toDate.ToString("yyyy-MM-dd"),
                PreviousMtdDate = preToDate.ToString("yyyy-MM-dd")
            };

            var jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                var path = $"{AppConfigService.Config.Path}/{DateTime.Now:yyyyMMdd}";
                Directory.CreateDirectory(path);
                File.WriteAllBytes($"{path}/dhwl_chart-{title}-{DateTime.Now:yyyyMMdd}.png", imageBytes);
                log.Log($"File dhwl_chart-{title}-{DateTime.Now:yyyyMMdd}.png is created!");
            }
            else
            {
                var text = await response.Content.ReadAsStringAsync();
                log.Log($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase} - {text}");
            }
        }
    }
}