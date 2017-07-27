using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetradarVirtualSportsMonthly.Business.Models;
using Epplus.Mapper.Extensions;
using OfficeOpenXml;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly DataService dataService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly DateService dateService;

        public ExcelServiceImp(ExcelPackage excelPackage, DataService dataService, ApplicationConfigurationService appConfigService, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dataService = dataService;
            this.appConfigService = appConfigService;
            this.dateService = dateService;
        }

        public ExcelService Export()
        {
            var fromDate = dateService.FromDate;
            var toDate = dateService.ToDate;
            var monthlySheet = excelPackage.Workbook.Worksheets["BetradarVS_Monthly"];
            var totalSheet = excelPackage.Workbook.Worksheets["Total"];
            var data = dataService.QueryMonthly(fromDate, toDate);
            var monthlyData = data.Item1;
            var totalData = data.Item2;

            RenderHeaderInfomation(monthlySheet, fromDate, toDate);
            if (monthlyData.Any())
            {
                RenderContent(monthlySheet, totalSheet, monthlyData, totalData);
            }
            else
            {
                RenderNoData(monthlySheet);
            }

            ClearTemplateSheet(totalSheet);

            return this;
        }

        private static void RenderContent(
            ExcelWorksheet monthlySheet,
            ExcelWorksheet totalSheet,
            IEnumerable<GrossWinningMonthly> monthlyData,
            IEnumerable<GrossWinningMonthly> totalData)
        {
            var beginRowIndex = 4;
            var lastRowIndexMonthlySheet = monthlyData.Count() + beginRowIndex;
            var lastRowIndexTotalSheet = totalData.Count() + beginRowIndex;

            monthlySheet.ApplyVertical(monthlyData);
            totalSheet.ApplyVertical(totalData);
            totalSheet.Cells[beginRowIndex, 1, lastRowIndexTotalSheet, 9].Copy(monthlySheet.Cells[lastRowIndexMonthlySheet, 1]);

            monthlySheet.Cells[$"A{lastRowIndexMonthlySheet}:C{lastRowIndexMonthlySheet + totalData.Count() - 1}"].Merge = true;
            monthlySheet.Cells[$"A{lastRowIndexMonthlySheet}:C{lastRowIndexMonthlySheet + totalData.Count() - 1}"].Value = "Total in";
        }

        private static void RenderHeaderInfomation(ExcelWorksheet sheet, DateTime fromDate, DateTime toDate)
        {
            sheet.Cells["A2"].Value = $"{fromDate:MM/dd/yyyy} - {toDate:MM/dd/yyyy} GMT - 4";
        }

        private static void RenderNoData(ExcelWorksheet sheet)
        {
            sheet.Cells["A4"].Value = "There is no data displayed";
            sheet.Cells["A4:J4"].Merge = true;
        }

        private void ClearTemplateSheet(ExcelWorksheet totalSheet)
        {
            excelPackage.Workbook.Worksheets.Delete(totalSheet);
        }

        public string Save()
        {
            var fromDate = dateService.FromDate;
            var toDate = dateService.ToDate;
            var filename = $"Betradar_Virtual_Sports_Monthly_Report_{fromDate:MMddyyyy}_{toDate:MMddyyyy}.xlsx";
            var folder = $@"{appConfigService.Configurations.ExportDataDirectory}\{fromDate:yyyy}";
            var fullName = $@"{folder}\{filename}";
            Directory.CreateDirectory(folder);
            excelPackage.SaveAs(new FileInfo(fullName), appConfigService.Configurations.ExcelPasswordProtected);

            return fullName;
        }
    }
}