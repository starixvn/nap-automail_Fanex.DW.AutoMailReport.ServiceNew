using Fanex.MarginChart.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Fanex.MarginChart.Business.Services.Margin
{
    public interface ExcelMarginService
    {
        string Export(List<MarginModel> data, bool isDaily, string category, string reportId);

        int LastRowIndex { get; set; }
    }

    public class ExcelMarginServiceImp : ExcelMarginService
    {
        private ExcelPackage excelPackage;
        private readonly DateTimeServiceProvider dateService;

        private const int StartRowIndex = 3;

        public int LastRowIndex
        {
            get; set;
        }

        public ExcelMarginServiceImp(
            ExcelPackage excelPackage,
            DateTimeServiceProvider dateService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
        }

        public string Export(List<MarginModel> data, bool isDaily, string category, string reportId)
        {
            excelPackage = new ExcelPackage(new FileInfo($"Templates/Margin-Letstalk.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Margin"];
            LastRowIndex = StartRowIndex + data.Count;

            RenderTitle(template, isDaily, category, reportId);
            RenderData(template, data);

            return Save(isDaily, category);
        }

        private void RenderTitle(ExcelWorksheet template, bool isDaily, string category, string reportId)
        {
            var todate = dateService.DateTime.ToDate;
            template.Cells["B2"].Value = isDaily
                ? $"{todate:yyyyMMdd} - {category.ToUpper()} MARGIN"
                : $"MTD - {category.ToUpper()} MARGIN";
            template.Cells["E1"].Value = reportId;
        }

        private void RenderData(ExcelWorksheet template, List<MarginModel> data)
        {
            template.Cells["B4"].LoadFromCollection(data, false);
        }

        private string Save(bool isDaily, string category)
        {
            var date = dateService.DateTime.ToDate;
            var fileName = isDaily
                ? $"Daily-{category}-Margin-{date:yyyyMMdd}.xlsx"
                : $"MTD-{category}-Margin-{date:yyyyMMdd}.xlsx";

            var path = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}\{category}";
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