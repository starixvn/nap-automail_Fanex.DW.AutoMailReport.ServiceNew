using Fanex.SoccerPerformance.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface ExcelService
    {
        List<string> Execute(Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> yesterdayData, Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> mtdData);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet yesterdaySheet;
        private readonly ExcelWorksheet mtdSheet;

        private readonly DateServiceProvider dateServiceProvider;
        private readonly ImageService imageService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateServiceProvider dateServiceProvider, ImageService imageService)
        {
            this.excelPackage = excelPackage;
            this.dateServiceProvider = dateServiceProvider;
            this.imageService = imageService;

            yesterdaySheet = excelPackage.Workbook.Worksheets["Daily"];
            mtdSheet = excelPackage.Workbook.Worksheets["MTD"];
        }

        public List<string> Execute(Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> yesterdayData, Tuple<IEnumerable<SoccerModel>, IEnumerable<SoccerModel>> mtdData)
        {
            const int rowIndex1 = 20;
            const int rowIndex2 = 38;
            const int rowHeight = 8;

            yesterdaySheet.Cells["A3"].LoadFromCollection(yesterdayData.Item1, false);
            yesterdaySheet.Cells["A44"].LoadFromCollection(yesterdayData.Item2, false);
            yesterdaySheet.InsertRow(rowIndex1, 1);
            yesterdaySheet.Row(rowIndex1).Height = rowHeight;
            yesterdaySheet.InsertRow(rowIndex2, 1);
            yesterdaySheet.Row(rowIndex2).Height = rowHeight;

            mtdSheet.Cells["A3"].LoadFromCollection(mtdData.Item1, false);
            mtdSheet.Cells["A44"].LoadFromCollection(mtdData.Item2, false);
            mtdSheet.InsertRow(rowIndex1, 1);
            mtdSheet.Row(rowIndex1).Height = rowHeight;
            mtdSheet.InsertRow(rowIndex2, 1);
            mtdSheet.Row(rowIndex2).Height = rowHeight;

            SetTitle();

            var excelPath = SaveExceFile();
            var imagePaths = imageService.SaveImage(excelPath);

            return imagePaths;
        }

        private void SetTitle()
        {
            yesterdaySheet.Cells["A1"].Value = $"Daily Soccer Performance by Bettypes on {dateServiceProvider.Date.Yesterday:yyyy/MM/dd}";
            mtdSheet.Cells["A1"].Value = $"Month-To-Date Soccer Performance by Bettypes from {dateServiceProvider.Date.StartDate:yyyy/MM/dd} to {dateServiceProvider.Date.EndDate:yyy/MM/dd}";
        }
      
        private string SaveExceFile()
        {
            var date = dateServiceProvider.Date.Yesterday;
            var fileName = $"Soccer_Performance_{date:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), string.Empty);

            return path;
        }
    }
}
