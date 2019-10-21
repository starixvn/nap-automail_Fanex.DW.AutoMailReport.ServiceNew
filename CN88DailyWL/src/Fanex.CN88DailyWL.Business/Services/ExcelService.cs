using Fanex.CN88DailyWL.Business.Models;
using OfficeOpenXml;
using System;
using System.IO;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface ExcelService
    {
        string Execute(Tuple<CN88Model, CN88Model> data);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet template;
        private readonly DateService dateService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;

            template = excelPackage.Workbook.Worksheets["Template"];
        }

        public string Execute(Tuple<CN88Model, CN88Model> data)
        {
            RenderDailyEN(data.Item1);
            RenderDailyCN(data.Item1);

            RenderPeriodEN(data.Item2);
            RenderPeriodCN(data.Item2);

            return SaveExceFile();
        }

        private void RenderDailyEN(CN88Model data)
        {
            template.Cells["C2"].Value = $"   [{data.Date}]";
            template.Cells["E3"].Value = data.ActiveMember;
            template.Cells["E4"].Value = data.BetCount;
            template.Cells["E5"].Value = data.MemberTO;
            template.Cells["E6"].Value = data.NetTO;
            template.Cells["E7"].Value = data.MemberWL;
            template.Cells["E8"].Value = data.CompanyTO_Live;
            template.Cells["E9"].Value = data.CompanyTO_NonLive;
            template.Cells["E10"].Value = data.CompanyTO_Mix;
            template.Cells["E11"].Value = data.CompanyTO;
            template.Cells["E12"].Value = data.CompanyIncome_Live;
            template.Cells["E13"].Value = data.CompanyIncome_NonLive;
            template.Cells["E14"].Value = data.CompanyIncome_Mix;
            template.Cells["E15"].Value = data.CompanyIncome;
            template.Cells["E16"].Value = data.CompanyMargin;
        }

        private void RenderDailyCN(CN88Model data)
        {
            template.Cells["C38"].Value = $"   [{data.Date}]";
            template.Cells["E39"].Value = data.ActiveMember;
            template.Cells["E40"].Value = data.BetCount;
            template.Cells["E41"].Value = data.MemberTO;
            template.Cells["E42"].Value = data.NetTO;
            template.Cells["E43"].Value = data.MemberWL;
            template.Cells["E44"].Value = data.CompanyTO_Live;
            template.Cells["E45"].Value = data.CompanyTO_NonLive;
            template.Cells["E46"].Value = data.CompanyTO_Mix;            
            template.Cells["E47"].Value = data.CompanyTO;
            template.Cells["E48"].Value = data.CompanyIncome_Live;
            template.Cells["E49"].Value = data.CompanyIncome_NonLive;
            template.Cells["E50"].Value = data.CompanyIncome_Mix;            
            template.Cells["E51"].Value = data.CompanyIncome;
            template.Cells["E52"].Value = data.CompanyMargin;
        }

        private void RenderPeriodEN(CN88Model data)
        {
            template.Cells["C19"].Value = $"   [Current Period: {data.Date}]";
            template.Cells["E20"].Value = data.ActiveMember;
            template.Cells["E21"].Value = data.BetCount;
            template.Cells["E22"].Value = data.MemberTO;
            template.Cells["E23"].Value = data.NetTO;
            template.Cells["E24"].Value = data.MemberWL;
            template.Cells["E25"].Value = data.CompanyTO_Live;
            template.Cells["E26"].Value = data.CompanyTO_NonLive;
            template.Cells["E27"].Value = data.CompanyTO_Mix;
            template.Cells["E28"].Value = data.CompanyTO;
            template.Cells["E29"].Value = data.CompanyIncome_Live;
            template.Cells["E30"].Value = data.CompanyIncome_NonLive;
            template.Cells["E31"].Value = data.CompanyIncome_Mix;            
            template.Cells["E32"].Value = data.CompanyIncome;
            template.Cells["E33"].Value = data.CompanyMargin;
        }

        private void RenderPeriodCN(CN88Model data)
        {
            template.Cells["C55"].Value = $"   [本期: {data.Date}]";
            template.Cells["E56"].Value = data.ActiveMember;
            template.Cells["E57"].Value = data.BetCount;
            template.Cells["E58"].Value = data.MemberTO;
            template.Cells["E59"].Value = data.NetTO;
            template.Cells["E60"].Value = data.MemberWL;
            template.Cells["E61"].Value = data.CompanyTO_Live;
            template.Cells["E62"].Value = data.CompanyTO_NonLive;
            template.Cells["E63"].Value = data.CompanyTO_Mix;
            template.Cells["E64"].Value = data.CompanyTO;
            template.Cells["E65"].Value = data.CompanyIncome_Live;
            template.Cells["E66"].Value = data.CompanyIncome_NonLive;
            template.Cells["E67"].Value = data.CompanyIncome_Mix;
            template.Cells["E68"].Value = data.CompanyIncome;
            template.Cells["E69"].Value = data.CompanyMargin;
        }

        private string SaveExceFile()
        {
            var date = dateService.WinLostDate;
            var fileName = $"CN88_Daily_WL{date:ddMMyyyy}.xlsx";
            var path = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), string.Empty);

            return path;
        }      
    }
}