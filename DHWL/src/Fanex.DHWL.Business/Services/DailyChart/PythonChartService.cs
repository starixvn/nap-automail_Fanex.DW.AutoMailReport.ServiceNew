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
        private readonly string apiUrl = $"{AppConfigService.Config.PythonChartApiUrl}/dhwl";
        private static readonly DateTimeServiceProvider dateTimeService = new DateTimeServiceProviderImp();

        private static readonly DateTimeService dateTimePreviousYearService =
            new DateTimePreviousYearServiceImp(dateTimeService);

        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        private static readonly Log log = new LogImp();

        public async Task<string> GenerateChart(
            Dictionary<int, double> sbFigure,
            Dictionary<int, double> totalFigure,
            ChartMtdFigureModel mtdFigure,
            string title,
            string chartName)
        {
            var toDate = dateTimeService.DateTime.ToDate;
            var preToDate = dateTimePreviousYearService.ToDate;
            var fromDate = dateTimeService.DateTime.FromDate;
            var path = Path.Combine(AppConfigService.Config.Path, DateTime.Now.ToString("yyyyMMdd"));
            Directory.CreateDirectory(path);
            var fileName = Path.Combine(path, $"dhwl_chart-{title}-{DateTime.Now:yyyyMMdd}.png");
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

            try
            {
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(fileName, imageBytes);
                }
                else
                {
                    var text = await response.Content.ReadAsStringAsync();
                    log.Log($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase} - {text}");
                    fileName = string.Empty;
                }
            }
            catch (TaskCanceledException ex)
            {
                log.Log($"HTTP Request Timeout: {ex.Message}");
                fileName = string.Empty;
            }
            catch (HttpRequestException ex)
            {
                log.Log($"HTTP Request Error: {ex.Message}");
                fileName = string.Empty;
            }
            catch (Exception ex)
            {
                log.Log($"Unexpected error in GenerateChart: {ex.Message}");
                fileName = string.Empty;
            }

            return fileName;
        }
    }
}