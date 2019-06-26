using System;
using System.Collections.Generic;
using System.IO;
using Fanex.Service.Core;
using MaxLossMonitoring.Business.Models;
using OfficeOpenXml;
using System.Linq;

namespace MaxLossMonitoring.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export(Tuple<List<MonitorList>, List<MonitorListRaw>> data);

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ApplicationConfigurationService appConfigService;
        private readonly ExcelPackage excelPackage;
        private readonly List<int> groupHeadLineIds = new List<int>();

        public ExcelServiceImp(ApplicationConfigurationService appConfigService, ExcelPackage excelPackage)
        {
            this.appConfigService = appConfigService;
            this.excelPackage = excelPackage;
        }

        public ExcelService Export(Tuple<List<MonitorList>, List<MonitorListRaw>> data)
        {
            var excelWorksheet = excelPackage.Workbook.Worksheets["SMA Loss Report"];
            var groupTemplate = excelPackage.Workbook.Worksheets["GroupTemplate"];
            var contentTemplate = excelPackage.Workbook.Worksheets["ContentTemplate"];

            excelPackage.Workbook.Worksheets.Delete(contentTemplate);
            excelPackage.Workbook.Worksheets.Delete(groupTemplate);

            var startRowIndex = 2;
            var startColumnIndex = 1;
            var endColumnIndex = 8;
            var rowIndex = 1;

            foreach (var group in data.Item1)
            {
                excelWorksheet.Cells[startRowIndex, startColumnIndex, startRowIndex, endColumnIndex].Value = $"{group.LevelNameForEmail} ({group.Rows.Count})";
                excelWorksheet.Cells[startRowIndex, startColumnIndex, startRowIndex, endColumnIndex].StyleID = groupTemplate.Cells[group.LevelId == 0 ? 1 : group.LevelId, 1].StyleID;
                excelWorksheet.Cells[startRowIndex, startColumnIndex, startRowIndex, endColumnIndex].Merge = true;

                startRowIndex++;
                foreach (var row in group.Rows)
                {
                    excelWorksheet.Cells[startRowIndex, 1].Value = rowIndex;
                    AssignBodyRowContent(excelWorksheet, startRowIndex, row, false);

                    startRowIndex++;
                    rowIndex++;

                    if (row.GroupHeadlineId > 0)
                    {
                        startRowIndex = AssignMemberUnderGroupHeadline(data, excelWorksheet, startRowIndex, row);
                        groupHeadLineIds.Add(row.GroupHeadlineId);
                    }
                }

                FormatBodyContent(data, excelWorksheet, contentTemplate, startRowIndex, startColumnIndex, endColumnIndex, group);                
            }

            return this;
        }

        private void FormatBodyContent(
            Tuple<List<MonitorList>, List<MonitorListRaw>> data, 
            ExcelWorksheet excelWorksheet, 
            ExcelWorksheet contentTemplate, 
            int startRowIndex, 
            int startColumnIndex, 
            int endColumnIndex,             
            MonitorList group)
        {
            var numberOfMemberUnderGroupHeadline = data.Item2.Count(s => groupHeadLineIds.Contains(s.GroupHeadlineId));
            for (var columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
            {
                excelWorksheet.Cells[startRowIndex - group.Rows.Count - numberOfMemberUnderGroupHeadline, columnIndex, startRowIndex - 1, columnIndex]
                    .StyleID = contentTemplate.Cells[2, columnIndex].StyleID;
            }

            groupHeadLineIds.Clear();
        }

        private static int AssignMemberUnderGroupHeadline(Tuple<List<MonitorList>, List<MonitorListRaw>> data, ExcelWorksheet excelWorksheet, int startRowIndex, MonitorListRaw row)
        {
            var rowsByGroupHeadiLine = data.Item2.Where(s => s.GroupHeadlineId == row.GroupHeadlineId).ToList();
            foreach (var subRow in rowsByGroupHeadiLine)
            {
                AssignBodyRowContent(excelWorksheet, startRowIndex, subRow, true);
                startRowIndex++;
            }

            return startRowIndex;
        }

        private static void AssignBodyRowContent(ExcelWorksheet excelWorksheet, int startRowIndex, MonitorListRaw row, bool isMemberGroupHeadline)
        {
            var space = isMemberGroupHeadline ? "     " : string.Empty;
            excelWorksheet.Cells[startRowIndex, 2].Value = space + row.Username;
            excelWorksheet.Cells[startRowIndex, 3].Value = row.RoleNameForEmail;
            excelWorksheet.Cells[startRowIndex, 4].Value = row.Currency;
            excelWorksheet.Cells[startRowIndex, 5].Value = row.YesterdayBalance;
            excelWorksheet.Cells[startRowIndex, 6].Value = row.TotalBalance;
            excelWorksheet.Cells[startRowIndex, 7].Value = row.MaxLoss;
            excelWorksheet.Cells[startRowIndex, 8].Value = row.PercentageLoss / 100;
        }

        public string Save()
        {
            var fileName = $"List_Of_SMA_Reaching_Allowable_Max_Loss_For_{SystemTime.Now:yyyy_MM_dd}.xlsx";
            var path = $@"{appConfigService.Configurations.ExportDataDirectory}\{SystemTime.Now:yyyy}\{SystemTime.Now:MM}";
            Directory.CreateDirectory(path);
            var fullPath = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(fullPath), appConfigService.Configurations.ExcelPasswordProtected);

            return fullPath;
        }
    }
}