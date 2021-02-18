using Fanex.TopLicensee.Service.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExcelTopLicenseeMarginService
    {
        string Export(string type, Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data);

        int LastRowIndex { get; set; }
    }

    public class ExcelTopLicenseeMarginServiceImp : ExcelTopLicenseeMarginService
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExcelTopLicenseeMarginServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public int LastRowIndex { get; set; }

        public string Export(string type, Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data)
        {
            ExportContent(type, data);
            return SaveCreatedFile(type);
        }

        private void ExportContent(string type, Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data)
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/TopLicensee-ALL.xlsx"));
            var sheet = excelPackage.Workbook.Worksheets["Template"];

            if (type.Equals("Daily"))
            {
                sheet.Cells["B2"].Value = $"{dateService.ToDate: yyyyMMdd} - MARGIN";
            }
            else
            {
                sheet.Cells["B2"].Value = $"MTD - MARGIN";
            }

            sheet.Cells["B4"].LoadFromCollection(data.Item2, false);
            LastRowIndex = data.Item2.Count + 3;

            var currentIndex = 4;
            var index = 1;
            data.Item2.GroupBy(g => new { g.LicenseeGroup })
                .ToList()
                .ForEach(group =>
                {
                    var cnt = group.Count();
                    sheet.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Merge = true;
                    sheet.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Value = index;
                    sheet.Cells[currentIndex, 3, currentIndex + cnt - 1, 3].Merge = true;

                    for (var columnIndex = 5; columnIndex <= 13; columnIndex++)
                    {
                        sheet.Cells[currentIndex, columnIndex, currentIndex + cnt - 1, columnIndex].Merge = true;
                    }

                    currentIndex += cnt;
                    index++;
                });
        }

        private string SaveCreatedFile(string type)
        {
            var rptDate = dateService.ToDate;
            var fileName = $"Top_Licensees_Margin_{type}_For_{rptDate:yyyyMMdd}.xlsx";
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