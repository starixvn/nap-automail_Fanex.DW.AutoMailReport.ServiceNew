using Fanex.TopLicensee.Service.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExcelTopLicenseeMarginService
    {
        string Export(string type, Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data);
    }

    public class ExcelTopLicenseeMarginServiceImp : ExcelTopLicenseeMarginService
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExcelTopLicenseeMarginServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public string Export(string type, Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data)
        {
            RefineData(data);
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

            sheet.Cells["B4"].LoadFromCollection(data.Item1, false);
            sheet.Cells["B16"].LoadFromCollection(data.Item2, false);
        }

        private void RefineData(Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> data)
        {
            int count = 1;
            data.Item1.ForEach(d =>
            {
                d.Order = count;
                count++;
            });

            count = 1;
            data.Item2.ForEach(d =>
            {
                d.Order = count;
                count++;
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