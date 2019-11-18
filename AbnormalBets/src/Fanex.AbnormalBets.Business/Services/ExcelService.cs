using Fanex.AbnormalBets.Business.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private readonly ExcelWorksheet detailTemplate;

        public ExcelServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            summaryTemplate = excelPackage.Workbook.Worksheets["Summary"];
            detailTemplate = excelPackage.Workbook.Worksheets["Detail"];
            excelPackage.Workbook.Worksheets.Delete(detailTemplate);
        }

        public string Export(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data)
        {
            GenSummary(data.Item1);
            GenDetail(data);
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

        private void GenDetail(Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data)
        {
            var count = 1;
            data.Item2
                .GroupBy(g => new { g.Sport, g.League, g.Match, g.KickOffTime, g.Bettype, g.Betteam, g.Reason, g.FinalScore, g.Home })
                .ToList()
                .ForEach(row => 
                {
                    var sheetName = count > 1
                        ? $"{row.Key.Home} ({count})"
                        : $"{row.Key.Home}";

                    excelPackage.Workbook.Worksheets.Add(sheetName);
                    ExcelWorksheet home = detailTemplate;
                    home.Cells["B1"].Value = row.Key.Sport;
                    home.Cells["B2"].Value = row.Key.League;
                    home.Cells["B3"].Value = row.Key.Match;
                    //home.Cells["B4"].Value = row.Key.ke;
                    count++;
                });
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