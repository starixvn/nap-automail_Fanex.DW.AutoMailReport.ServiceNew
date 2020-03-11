using Fanex.AbnormalBets.Business.Models.DailyCSD;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface ExcelCSDService
    {
        string Execute(List<AbnormalBetDTOModel> data, DateTime date);
    }

    public class ExcelCSDServiceImp : ExcelCSDService
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet template;

        public ExcelCSDServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            template = excelPackage.Workbook.Worksheets["Daily Abnormal Bet"];
        }

        public string Execute(List<AbnormalBetDTOModel> data, DateTime date)
        {
            var rowIndex = 3;
            template.Cells["B3"].LoadFromCollection(data, false);
            for (var columnIndex = 2; columnIndex <= 17; columnIndex++)
            {
                template.Cells[rowIndex, columnIndex, data.Count + rowIndex - 1, columnIndex].StyleID = template.Cells[rowIndex, columnIndex].StyleID;
                template.Column(columnIndex).AutoFit(17);
            }

            template.Column(10).AutoFit(17, 50);
            template.Column(12).AutoFit(17, 50);
            template.Column(17).AutoFit(22, 50);

            var fileName = $"Abnormal_Bet_For_{date:yyyy_MM_dd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }
    }
}