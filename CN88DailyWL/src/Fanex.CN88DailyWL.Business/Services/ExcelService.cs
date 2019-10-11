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
            template.Cells["E8"].Value = data.CompanyTO;
            template.Cells["E9"].Value = data.CompanyIncome_Live;
            template.Cells["E10"].Value = data.CompanyIncome_NonLive;
            template.Cells["E11"].Value = data.CompanyIncome_Mix;
            template.Cells["E12"].Value = data.CompanyIncome;
            template.Cells["E13"].Value = data.CompanyMargin;
        }

        private void RenderDailyCN(CN88Model data)
        {
            template.Cells["C32"].Value = $"   [{data.Date}]";
            template.Cells["E33"].Value = data.ActiveMember;
            template.Cells["E34"].Value = data.BetCount;
            template.Cells["E35"].Value = data.MemberTO;
            template.Cells["E36"].Value = data.NetTO;
            template.Cells["E37"].Value = data.MemberWL;
            template.Cells["E38"].Value = data.CompanyTO;
            template.Cells["E39"].Value = data.CompanyIncome_Live;
            template.Cells["E40"].Value = data.CompanyIncome_NonLive;
            template.Cells["E41"].Value = data.CompanyIncome_Mix;            
            template.Cells["E42"].Value = data.CompanyIncome;
            template.Cells["E43"].Value = data.CompanyMargin;
        }

        private void RenderPeriodEN(CN88Model data)
        {
            template.Cells["C16"].Value = $"   [Current Period: {data.Date}]";
            template.Cells["E17"].Value = data.ActiveMember;
            template.Cells["E18"].Value = data.BetCount;
            template.Cells["E19"].Value = data.MemberTO;
            template.Cells["E20"].Value = data.NetTO;
            template.Cells["E21"].Value = data.MemberWL;
            template.Cells["E22"].Value = data.CompanyTO;
            template.Cells["E23"].Value = data.CompanyIncome_Live;
            template.Cells["E24"].Value = data.CompanyIncome_NonLive;
            template.Cells["E25"].Value = data.CompanyIncome_Mix;            
            template.Cells["E26"].Value = data.CompanyIncome;
            template.Cells["E27"].Value = data.CompanyMargin;
        }

        private void RenderPeriodCN(CN88Model data)
        {
            template.Cells["C46"].Value = $"   [本期: {data.Date}]";
            template.Cells["E47"].Value = data.ActiveMember;
            template.Cells["E48"].Value = data.BetCount;
            template.Cells["E49"].Value = data.MemberTO;
            template.Cells["E50"].Value = data.NetTO;
            template.Cells["E51"].Value = data.MemberWL;
            template.Cells["E52"].Value = data.CompanyTO;
            template.Cells["E53"].Value = data.CompanyIncome_Live;
            template.Cells["E54"].Value = data.CompanyIncome_NonLive;
            template.Cells["E55"].Value = data.CompanyIncome_Mix;
            template.Cells["E56"].Value = data.CompanyIncome;
            template.Cells["E57"].Value = data.CompanyMargin;
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