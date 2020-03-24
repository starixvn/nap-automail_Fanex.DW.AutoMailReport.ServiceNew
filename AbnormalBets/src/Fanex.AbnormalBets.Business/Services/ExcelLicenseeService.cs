using Fanex.AbnormalBets.Business.Models.Licensee;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface ExcelLicenseeService
    {
        string Execute(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>, List<UserLocationModel>> data);
    }

    public class ExcelLicenseeServiceImp : ExcelLicenseeService
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet summaryTemplate;
        private ExcelWorksheet detailTemplate;
        private ExcelWorksheet ipTemplate;

        public ExcelLicenseeServiceImp(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public string Execute(string site, DateTime rptDate, Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>, List<UserLocationModel>> data)
        {
            excelPackage = new ExcelPackage(new FileInfo(@"Templates/AbnormalBet.xlsx"));
            Init();
            GenSummary(data.Item1);
            GenDetail(data.Item2);
            GetIPSummary(data.Item3);

            return SaveExceFile(site, rptDate);
        }

        private void Init()
        {
            summaryTemplate = excelPackage.Workbook.Worksheets["Summary"];
            detailTemplate = excelPackage.Workbook.Worksheets["Detail"];
            ipTemplate = excelPackage.Workbook.Worksheets["IP Summary"];
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
            var detail = excelPackage.Workbook.Worksheets["Detail"];
            detail.Cells["A2"].LoadFromCollection(data, false);

            for (int columnIndex = 1; columnIndex <= 22; columnIndex++)
            {
                detail.Cells[2, columnIndex, data.Count + 1, columnIndex].StyleID = detail.Cells[2, columnIndex].StyleID;
                detail.Column(columnIndex).AutoFit(20);
            }           
        }

        private void GetIPSummary(List<UserLocationModel> data)
        {
            var rowIndex = 2;
            ipTemplate.Cells["A2"].LoadFromCollection(data, false);
            for (var columnIndex = 1; columnIndex <= 5; columnIndex++)
            {
                ipTemplate.Cells[rowIndex, columnIndex, data.Count + rowIndex - 1, columnIndex].StyleID = ipTemplate.Cells[rowIndex, columnIndex].StyleID;
                ipTemplate.Column(columnIndex).AutoFit(20);
            }
        }

        private string SaveExceFile(string site, DateTime rptDate)
        {
            excelPackage.Workbook.Worksheets[1].Select();
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