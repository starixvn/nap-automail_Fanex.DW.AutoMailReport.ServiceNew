using OfficeOpenXml;
using System.Data;
using System.IO;

namespace Fanex.MarginChart.Business.Services
{
    public interface DailyExcelService
    {
        string Export(DataTable dailyRawData);
    }

    public class DailyExcelServiceImp : DailyExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;
        private readonly DataHouseIncomeService dataHouseIncomeService;
        private readonly DataService dataService;
        private readonly DataLastMonthService dataLastMonthService;

        private const int StartRowIndex = 7;

        public DailyExcelServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth,
            DataHouseIncomeService dataHouseIncomeService,
            DataService dataService,
            DataLastMonthService dataLastMonthService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
            this.dataHouseIncomeService = dataHouseIncomeService;
            this.dataService = dataService;
            this.dataLastMonthService = dataLastMonthService;
        }

        public string Export(DataTable dailyRawData)
        {
            var dataForExcel = dataHouseIncomeService.CalTotalByProductForExcel(dailyRawData);
            excelPackage = new ExcelPackage(new FileInfo($"Templates/MarginChart-Letstalk.xlsx"));
            var dailyTemplate = excelPackage.Workbook.Worksheets["Letstalk"];

            return Save();
        }

        private string Save()
        {
            var date = dateService.DateTime.ToDate;
            var fileName = $"MarginChart_{date:yyyy_MM_dd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);

            return path;
        }
    }
}