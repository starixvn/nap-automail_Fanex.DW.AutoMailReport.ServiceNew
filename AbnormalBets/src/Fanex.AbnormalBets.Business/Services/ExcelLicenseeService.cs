using Fanex.AbnormalBets.Business.Models.DailyLicensee;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface ExcelLicenseeService
    {
        string Execute(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data);
    }

    public class ExcelLicenseeServiceImp : ExcelLicenseeService
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet summaryTemplate;
        private ExcelWorksheet detailTemplate;

        public ExcelLicenseeServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public string Execute(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> data)
        {
            excelPackage = new ExcelPackage(new FileInfo(@"Templates/AbnormalBet.xlsx"));
            Init();
            GenSummary(data.Item1);
            GenDetail(data.Item2);
            return SaveExceFile(site, rptDate);
        }

        private void Init()
        {
            summaryTemplate = excelPackage.Workbook.Worksheets["Summary"];
            detailTemplate = excelPackage.Workbook.Worksheets["Detail"];
            excelPackage.Workbook.Worksheets.Delete(detailTemplate);
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

        private void GenDetail(List<DetailBetDTOModel> data)
        {
            var count = 1;
            data.GroupBy(g => new { g.Sport, g.League, g.Match, g.KickOffTime, g.Bettype, g.Betteam, g.Reason, g.FinalScore, g.Home, g.Live })
                .ToList()
                .ForEach(row =>
                {
                    var sheetName = GetSheetName(row.Key.Home, count);
                    var detail = detailTemplate;
                    detail.Cells["B1"].Value = row.Key.Sport;
                    detail.Cells["B2"].Value = row.Key.League;
                    detail.Cells["B3"].Value = row.Key.Match;
                    detail.Cells["B4"].Value = row.Key.KickOffTime;
                    detail.Cells["B5"].Value = row.Key.Live;
                    detail.Cells["B6"].Value = row.Key.Bettype;
                    detail.Cells["B7"].Value = row.Key.Betteam;
                    detail.Cells["B8"].Value = row.Key.Reason;
                    detail.Cells["B9"].Value = row.Key.FinalScore;

                    count++;
                    var betDetails = new List<BetDetailModel>();
                    row.ToList()
                    .ForEach(bet =>
                    {
                        betDetails.Add(new BetDetailModel
                        {
                            Username = bet.Username,
                            Odds = bet.Odds,
                            CurrencyName = bet.CurrencyName,
                            Stake = bet.Stake,
                            Hdp = bet.Hdp,
                            LiveHomeScore = bet.LiveHomeScore,
                            LiveAwayScore = bet.LiveAwayScore,
                            TransDate = bet.TransDate,
                            TransId = bet.TransId
                        });
                    });

                    var lastRowIndex = detail.Dimension.End.Row;
                    if (lastRowIndex > 12)
                    {
                        detail.DeleteRow(13, lastRowIndex);
                    }

                    detail.Cells["B12"].LoadFromCollection(betDetails, false);
                    for (int i = 2; i <= 10; i++)
                    {
                        detail.Cells[12, i, 11 + betDetails.Count, i].StyleID = detail.Cells[12, i].StyleID;
                    }

                    excelPackage.Workbook.Worksheets.Add(sheetName, detail);
                });
        }

        private string GetSheetName(string home, int count)
        {
            var index = home.Length > 27 ? 27 : home.Length;
            var sheetname = home.Substring(0, index);
            return count > 1
                ? $"{sheetname}({count})"
                : sheetname;
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