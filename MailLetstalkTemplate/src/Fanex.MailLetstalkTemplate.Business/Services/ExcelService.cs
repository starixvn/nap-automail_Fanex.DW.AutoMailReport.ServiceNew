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

        public ExcelServiceImp(ExcelPackage excelPackage, DateRangeServiceProfier dateRangeServiceProfier, MailFormatProvider mailFormatProvider)
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
                var totalSetting = AppConfigService.Config.IsEnabledSectionTotal ? AppConfigService.Config.TotalSettings[index] : new TotalSetting();
                var template = excelPackage.Workbook.Worksheets[AppConfigService.Config.SheetExcelTemplateName[index]];
                var startRowIndex = AppConfigService.Config.StartExelRowIndex;
                AssignTotalDataRow(dataTable, totalSetting);

                var maxRowIndex = dataTable.Rows.Count + startRowIndex - 1;
                template.Cells[AppConfigService.Config.CellAddressRender[index]].LoadFromDataTable(dataTable, false);
                FormatHeaderTitle(index, template);

                if (dataTable.Rows.Count == 0)
                {
                    FormatNoData(index, template, dataTable.Columns.Count);
                }
                else
                {
                    FormatContentDataRow(dataTable, startRowIndex, maxRowIndex, template);
                    FormatSectionTotal(dataTable, totalSetting, template, maxRowIndex);
                }

                index++;
            }

            return Save();
        }

        private static void FormatNoData(int index, ExcelWorksheet template, int numberOfColumns)
        {
            template.Cells[AppConfigService.Config.CellAddressRender[index]].Value = "There is no data";
            template.Cells[AppConfigService.Config.StartExelRowIndex, AppConfigService.Config.StartExcelColumnIndex, AppConfigService.Config.StartExelRowIndex, numberOfColumns + 1].Merge = true;
            template.Cells[AppConfigService.Config.StartExelRowIndex, AppConfigService.Config.StartExcelColumnIndex, AppConfigService.Config.StartExelRowIndex, numberOfColumns + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        }

        private static void FormatContentDataRow(DataTable dataTable, int startRowIndex, int maxRowIndex, ExcelWorksheet template)
        {
            for (int columnIndex = AppConfigService.Config.StartExcelColumnIndex; columnIndex < dataTable.Columns.Count + AppConfigService.Config.StartExcelColumnIndex; columnIndex++)
            {
                template.Cells[startRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
            }

            for (int rowIndex = startRowIndex; rowIndex <= maxRowIndex; rowIndex++)
            {
                template.Row(rowIndex).Height = 18;
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

            return path;
        }
    }
}