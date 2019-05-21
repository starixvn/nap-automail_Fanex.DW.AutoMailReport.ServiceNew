using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Epplus.Mapper.Extensions;
using OfficeOpenXml;

namespace BetradarVirtualSportsDaily.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly DataService dataDailyService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly DateService dateService;

        public ExcelServiceImp(ExcelPackage excelPackage, DataService dataDailyService, ApplicationConfigurationService appConfigService, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dataDailyService = dataDailyService;
            this.appConfigService = appConfigService;
            this.dateService = dateService;
        }

        public ExcelService Export()
        {
            var date = dateService.Date;
            var dailySheet = excelPackage.Workbook.Worksheets["BetradarVS_Daily"];
            var totalSheet = excelPackage.Workbook.Worksheets["Total"];
            var data = dataDailyService.QueryDaily(date);
            var dailyData = data.Item1;
            var totalData = data.Item2;

            RenderHeaderInfomation(dailySheet, date);
            if (dailyData.Any())
            {
                RenderContent(dailySheet, totalSheet, dailyData, totalData);
            }
            else
            {
                RenderNoData(dailySheet);
            }

            ClearTemplateSheet(totalSheet);

            return this;
        }

        private static void RenderContent(
            ExcelWorksheet dailySheet,
            ExcelWorksheet totalSheet,
            IEnumerable<Models.GrossWinningDaily> dailyData,
            IEnumerable<Models.GrossWinningDaily> totalData)
        {
            const int BeginRowIndex = 4;
            const int LastColumnIndex = 10;
            var lastRowIndexDailySheet = dailyData.Count() + BeginRowIndex;
            var lastRowIndexTotalSheet = totalData.Count() + BeginRowIndex;

            dailySheet.ApplyVertical(dailyData);
            totalSheet.ApplyVertical(totalData);
            totalSheet.Cells[BeginRowIndex, 1, lastRowIndexTotalSheet, LastColumnIndex].Copy(dailySheet.Cells[lastRowIndexDailySheet, 1]);

            dailySheet.AutoMergeRowsHaveSameValue($"A1:A{lastRowIndexDailySheet - 1}");            
            dailySheet.AutoMergeRowsHaveSameValue($"B1:B{lastRowIndexDailySheet - 1}");
            
            dailySheet.Cells[$"A{lastRowIndexDailySheet}:B{lastRowIndexDailySheet + totalData.Count() - 1}"].Merge = true;
            dailySheet.Cells[$"A{lastRowIndexDailySheet}:B{lastRowIndexDailySheet + totalData.Count() - 1}"].Value = "Total in";
        }

        private static void RenderHeaderInfomation(ExcelWorksheet sheet, DateTime date)
        {
            sheet.Cells["A2"].Value = $"{date:MM/dd/yyyy}";
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
            var date = dateService.Date;
            var filename = $"Betradar_Virtual_Sports_Daily_Report_{date:MMddyyyy}.xlsx";
            var folder = $@"{appConfigService.Configurations.ExportDataDirectory}\{date:yyyy}\{date:MM}";
            var fullName = $@"{folder}\{filename}";
            Directory.CreateDirectory(folder);
            excelPackage.SaveAs(new FileInfo(fullName), appConfigService.Configurations.ExcelPasswordProtected);

            return fullName;
        }
    }
}