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
            yesterdaySheet.Cells["A3"].LoadFromCollection(yesterdayData.Item1, false);
            yesterdaySheet.Cells["A38"].LoadFromCollection(yesterdayData.Item2, false);

            mtdSheet.Cells["A3"].LoadFromCollection(mtdData.Item1, false);
            mtdSheet.Cells["A38"].LoadFromCollection(mtdData.Item2, false);

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
