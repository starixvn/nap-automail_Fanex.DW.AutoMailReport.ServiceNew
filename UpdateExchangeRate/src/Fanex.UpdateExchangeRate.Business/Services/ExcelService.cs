using System;
using System.Collections.Generic;
using System.IO;
using Epplus.Mapper.Extensions;
using Fanex.UpdateExchangeRate.Business.Models;
using OfficeOpenXml;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export(List<CurrencyRate> data);

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ApplicationConfigurationService appConfigService;

        public ExcelServiceImp(ExcelPackage excelPackage, ApplicationConfigurationService appConfigService)
        {
            this.excelPackage = excelPackage;
            this.appConfigService = appConfigService;
        }

        public ExcelService Export(List<CurrencyRate> data)
        {
            var updatedRate = excelPackage.Workbook.Worksheets["Updated Rate"];
            updatedRate.Cells["A1"].Value = $"Weekly Currency Updated Rate on {DateTime.Now:yyyy-MM-dd}";
            updatedRate.ApplyVertical(data);
            return this;
        }

        public string Save()
        {
            var filename = $"Weekly_Currency_Updated_Rate_on_{DateTime.Now:yyyy-MM-dd}.xlsx ";
            var folder = $@"{appConfigService.Configurations.ExportDataDirectory}\{DateTime.Now:yyyy}\{DateTime.Now:MM}";
            var fullName = $@"{folder}\{filename}";
            Directory.CreateDirectory(folder);
            excelPackage.SaveAs(new FileInfo(fullName), appConfigService.Configurations.ExcelPasswordProtected);

            return fullName;
        }
    }
}