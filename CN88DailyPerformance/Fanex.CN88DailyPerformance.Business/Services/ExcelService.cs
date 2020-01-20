using Epplus.Mapper.Extensions;
using Fanex.CN88DailyPerformance.Service.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.CN88DailyPerformance.Service.Services
{
    public interface ExcelService
    {
        string ExportExcel(IEnumerable<DailyPerformanceModel> dailyPerformances, DateTime reportDate);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet dataSheet;

        public string ExportExcel(IEnumerable<DailyPerformanceModel> dailyPerformances, DateTime reportDate)
        {
            const int startRowIndex = 2;
            LoadAndCreateSheet();
            dataSheet.ApplyVertical(dailyPerformances, startRowIndex);
            ClearTemplate();

            return SaveCreatedFile(reportDate);
        }

        private string SaveCreatedFile(DateTime reportDate)
        {
            var fileName = $"{reportDate:yyyyMMdd}_CN88_Performance.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{reportDate:yyyy}\{reportDate:MM}";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }

        private void LoadAndCreateSheet()
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/CN88DailyPerformance.xlsx"));
            dataSheet = excelPackage.Workbook.Worksheets.Copy("DATA", "Daily Performance");
        }

        private void ClearTemplate()
        {
            excelPackage.Workbook.Worksheets.Delete("DATA");
        }
    }
}
