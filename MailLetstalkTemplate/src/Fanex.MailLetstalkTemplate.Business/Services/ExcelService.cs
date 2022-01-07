using Fanex.Common;
using Fanex.MailLetstalkTemplate.Business.Models;
using Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate;
using OfficeOpenXml;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(DataSet dataSet);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateRangeServiceProfier dateRangeServiceProfier;
        private readonly MailFormatProvider mailFormatProvider;
        private MailFormat mailFormat;

        public ExcelServiceImp(
            ExcelPackage excelPackage,
            DateRangeServiceProfier dateRangeServiceProfier,
            MailFormatProvider mailFormatProvider)
        {
            this.excelPackage = excelPackage;
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            this.mailFormatProvider = mailFormatProvider;
        }

        public string ExportExcel(DataSet dataSet)
        {
            mailFormat = mailFormatProvider.Format(AppConfigService.Config.DateRange);
            excelPackage = new ExcelPackage(new FileInfo($"Templates/{AppConfigService.Config.TemplateExcelFile}"));
            var index = 0;
            foreach (DataTable dataTable in dataSet.Tables)
            {
                var rowPerBatch = 500000;
                var totalBatch = dataTable.Rows.Count / rowPerBatch + ((dataTable.Rows.Count % rowPerBatch) > 0 ? 1 : 0);
                var sheetName = AppConfigService.Config.SheetExcelTemplateName[index];
                var temlate = excelPackage.Workbook.Worksheets[sheetName];

                if (dataTable.Rows.Count > 0)
                {
                    excelPackage.Workbook.Worksheets.Delete(sheetName);
                }

                for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
                {
                    var rowsOfBatch = dataTable.AsEnumerable().Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                    var subTable = rowsOfBatch.CopyToDataTable();
                    var totalSetting = AppConfigService.Config.IsEnabledSectionTotal ? AppConfigService.Config.TotalSettings[index] : new TotalSetting();
                    var newSheet = excelPackage.Workbook.Worksheets.Add($"{sheetName}-{batchIndex + 1}");
                    newSheet.View.ShowGridLines = false;
                    var startRowIndex = AppConfigService.Config.StartExelRowIndex;
                    newSheet.View.FreezePanes(startRowIndex, 3);
                    for (int row = 1; row < startRowIndex; row++)
                    {
                        for (int column = 1; column <= temlate.Dimension.Columns; column++)
                        {
                            temlate.Cells[row, column].Copy(newSheet.Cells[row, column]);
                            newSheet.Column(column).Width = temlate.Column(column).Width;
                        }
                    }

                    AssignTotalDataRow(subTable, totalSetting);

                    var maxRowIndex = subTable.Rows.Count + startRowIndex - 1;
                    newSheet.Cells[AppConfigService.Config.CellAddressRender[index]].LoadFromDataTable(subTable, false);
                    FormatHeaderTitle(index, newSheet);

                    if (subTable.Rows.Count == 0)
                    {
                        FormatNoData(index, newSheet, subTable.Columns.Count);
                    }
                    else
                    {
                        FormatContentDataRow(subTable, startRowIndex, maxRowIndex, newSheet, temlate);
                        FormatSectionTotal(subTable, totalSetting, newSheet, maxRowIndex);
                    }
                }

                index++;
            }

            return Save();
        }

        private static void FormatNoData(int index, ExcelWorksheet template, int numberOfColumns)
        {
            template.Cells[AppConfigService.Config.CellAddressRender[index]].Value = "There is no data";
            template.Cells[AppConfigService.Config.StartExelRowIndex, AppConfigService.Config.StartExcelColumnIndex, AppConfigService.Config.StartExelRowIndex, numberOfColumns].Merge = true;
            template.Cells[AppConfigService.Config.StartExelRowIndex, AppConfigService.Config.StartExcelColumnIndex, AppConfigService.Config.StartExelRowIndex, numberOfColumns].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        private static void FormatContentDataRow(DataTable dataTable, int startRowIndex, int maxRowIndex, ExcelWorksheet newSheet, ExcelWorksheet template)
        {
            for (int columnIndex = AppConfigService.Config.StartExcelColumnIndex; columnIndex < dataTable.Columns.Count + AppConfigService.Config.StartExcelColumnIndex; columnIndex++)
            {
                newSheet.Cells[startRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
            }

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                newSheet.Row(rowIndex).Height = 18;
            }
        }

        private void FormatHeaderTitle(int index, ExcelWorksheet template)
        {
            if (AppConfigService.Config.IsEnabledSectionHeader)
            {
                template.Cells["B2"].Value = mailFormat.Header();
                template.Cells["B3"].Value = $"Generated on {DateTime.Now:dd MMM yyyy hh:mm:ss tt}";
            }
        }

        private static void FormatSectionTotal(DataTable dataTable, TotalSetting totalSetting, ExcelWorksheet template, int maxRowIndex)
        {
            if (AppConfigService.Config.IsEnabledSectionTotal && totalSetting.IsEnabledSectionTotal && dataTable.Rows.Count > 0)
            {
                for (int columnIndex = AppConfigService.Config.StartExcelColumnIndex; columnIndex < dataTable.Columns.Count + AppConfigService.Config.StartExcelColumnIndex; columnIndex++)
                {
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Font.Bold = true;
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    template.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 241, 222));
                }

                template.Cells[maxRowIndex, totalSetting.StartMergeColumnIndex, maxRowIndex, totalSetting.EndMergeColumnIndex].Merge = true;
                template.Cells[maxRowIndex, totalSetting.StartMergeColumnIndex, maxRowIndex, totalSetting.EndMergeColumnIndex].Value = totalSetting.Label;
                template.Cells[maxRowIndex, totalSetting.StartMergeColumnIndex, maxRowIndex, totalSetting.EndMergeColumnIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
        }

        private static void AssignTotalDataRow(DataTable dataTable, TotalSetting totalSetting)
        {
            if (AppConfigService.Config.IsEnabledSectionTotal && totalSetting.IsEnabledSectionTotal && dataTable.Rows.Count > 0)
            {
                DataRow drTotal = dataTable.NewRow();
                foreach (var columnInex in totalSetting.SumColumnsIndex)
                {
                    drTotal[columnInex] = dataTable.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnInex]));
                }

                dataTable.Rows.Add(drTotal);
            }
        }

        private string Save()
        {
            if (excelPackage.Workbook.Worksheets.Count > 1)
            {
                excelPackage.Workbook.Worksheets[1].Select();
            }

            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange).ToDate;
            var fileName = mailFormat.FileName() + ".xlsx";
            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.Config.Password);
            excelPackage.Dispose();

            return path;
        }
    }
}