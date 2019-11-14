using Fanex.AbnormalBets.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface ExcelService
    {
        string Export(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet summaryTemplate;

        public ExcelServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            summaryTemplate = excelPackage.Workbook.Worksheets["Summary"];
        }

        public string Export(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data)
        {
            GenSummary(data.Item1);
            return SaveExceFile(site, rptDate);
        }

        private void GenSummary(List<SummaryBetDTOModel> bets)
        {
            var rowIndex = 4;
            summaryTemplate.Cells["B4"].LoadFromCollection(bets, false);
            for (var columnIndex = 2; columnIndex <= 12; columnIndex++)
            {
                summaryTemplate.Cells[rowIndex, columnIndex, bets.Count + rowIndex - 1, columnIndex].StyleID = summaryTemplate.Cells[rowIndex, columnIndex].StyleID;
                summaryTemplate.Column(columnIndex).AutoFit(20);
            }
        }

        private string SaveExceFile(string site, DateTime rptDate)
        {
            var siteName = site.ToLower() == "all" ? string.Empty : $"{site}_";
            var fileName = $"{siteName}Abnormal_Bet_Daily_Report_{rptDate:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{rptDate:yyyy}\{rptDate:MM}";
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