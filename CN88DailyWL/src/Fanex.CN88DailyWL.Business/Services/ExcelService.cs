using Fanex.CN88DailyWL.Business.Models;
using OfficeOpenXml;
using System;
using System.IO;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface ExcelService
    {
        string Execute(Tuple<ReportModel, ReportModel> data);
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

        public string Execute(Tuple<ReportModel, ReportModel> data)
        {
            RenderDailyEN(data.Item1);
            RenderDailyCN(data.Item1);

            RenderPeriodEN(data.Item2);
            RenderPeriodCN(data.Item2);

            return SaveExceFile();
        }

        private void RenderDailyEN(ReportModel data)
        {
            template.Cells["B2"].Value = $"   [{data.Date}]";
            template.Cells["C4"].Value = data.TPG.ActiveMember;
            template.Cells["C5"].Value = data.TPG.BetCount;
            template.Cells["C6"].Value = data.TPG.MemberTO;
            template.Cells["C7"].Value = data.TPG.NetTO;
            template.Cells["C8"].Value = data.TPG.MemberWL;
            template.Cells["C9"].Value = data.TPG.CompanyTO_Live;
            template.Cells["C10"].Value = data.TPG.CompanyTO_NonLive;
            template.Cells["C11"].Value = data.TPG.CompanyTO_Mix;
            template.Cells["C12"].Value = data.TPG.CompanyTO;
            template.Cells["C13"].Value = data.TPG.CompanyIncome_Live;
            template.Cells["C14"].Value = data.TPG.CompanyIncome_NonLive;
            template.Cells["C15"].Value = data.TPG.CompanyIncome_Mix;
            template.Cells["C16"].Value = data.TPG.CompanyIncome;
            template.Cells["C17"].Value = data.TPG.CompanyMargin;

            template.Cells["D4"].Value = data.XPG.ActiveMember;
            template.Cells["D5"].Value = data.XPG.BetCount;
            template.Cells["D6"].Value = data.XPG.MemberTO;
            template.Cells["D7"].Value = data.XPG.NetTO;
            template.Cells["D8"].Value = data.XPG.MemberWL;
            template.Cells["D9"].Value = data.XPG.CompanyTO_Live;
            template.Cells["D10"].Value = data.XPG.CompanyTO_NonLive;
            template.Cells["D11"].Value = data.XPG.CompanyTO_Mix;
            template.Cells["D12"].Value = data.XPG.CompanyTO;
            template.Cells["D13"].Value = data.XPG.CompanyIncome_Live;
            template.Cells["D14"].Value = data.XPG.CompanyIncome_NonLive;
            template.Cells["D15"].Value = data.XPG.CompanyIncome_Mix;
            template.Cells["D16"].Value = data.XPG.CompanyIncome;
            template.Cells["D17"].Value = data.XPG.CompanyMargin;

            template.Cells["E4"].Value = data.Total.ActiveMember;
            template.Cells["E5"].Value = data.Total.BetCount;
            template.Cells["E6"].Value = data.Total.MemberTO;
            template.Cells["E7"].Value = data.Total.NetTO;
            template.Cells["E8"].Value = data.Total.MemberWL;
            template.Cells["E9"].Value = data.Total.CompanyTO_Live;
            template.Cells["E10"].Value = data.Total.CompanyTO_NonLive;
            template.Cells["E11"].Value = data.Total.CompanyTO_Mix;
            template.Cells["E12"].Value = data.Total.CompanyTO;
            template.Cells["E13"].Value = data.Total.CompanyIncome_Live;
            template.Cells["E14"].Value = data.Total.CompanyIncome_NonLive;
            template.Cells["E15"].Value = data.Total.CompanyIncome_Mix;
            template.Cells["E16"].Value = data.Total.CompanyIncome;
            template.Cells["E17"].Value = data.Total.CompanyMargin;
        }

        private void RenderDailyCN(ReportModel data)
        {
            template.Cells["B36"].Value = $"   [{data.Date}]";
            template.Cells["C38"].Value = data.TPG.ActiveMember;
            template.Cells["C39"].Value = data.TPG.BetCount;
            template.Cells["C40"].Value = data.TPG.MemberTO;
            template.Cells["C41"].Value = data.TPG.NetTO;
            template.Cells["C42"].Value = data.TPG.MemberWL;
            template.Cells["C43"].Value = data.TPG.CompanyTO_Live;
            template.Cells["C44"].Value = data.TPG.CompanyTO_NonLive;
            template.Cells["C45"].Value = data.TPG.CompanyTO_Mix;            
            template.Cells["C46"].Value = data.TPG.CompanyTO;
            template.Cells["C47"].Value = data.TPG.CompanyIncome_Live;
            template.Cells["C48"].Value = data.TPG.CompanyIncome_NonLive;
            template.Cells["C49"].Value = data.TPG.CompanyIncome_Mix;            
            template.Cells["C50"].Value = data.TPG.CompanyIncome;
            template.Cells["C51"].Value = data.TPG.CompanyMargin;

            template.Cells["D38"].Value = data.XPG.ActiveMember;
            template.Cells["D39"].Value = data.XPG.BetCount;
            template.Cells["D40"].Value = data.XPG.MemberTO;
            template.Cells["D41"].Value = data.XPG.NetTO;
            template.Cells["D42"].Value = data.XPG.MemberWL;
            template.Cells["D43"].Value = data.XPG.CompanyTO_Live;
            template.Cells["D44"].Value = data.XPG.CompanyTO_NonLive;
            template.Cells["D45"].Value = data.XPG.CompanyTO_Mix;
            template.Cells["D46"].Value = data.XPG.CompanyTO;
            template.Cells["D47"].Value = data.XPG.CompanyIncome_Live;
            template.Cells["D48"].Value = data.XPG.CompanyIncome_NonLive;
            template.Cells["D49"].Value = data.XPG.CompanyIncome_Mix;
            template.Cells["D50"].Value = data.XPG.CompanyIncome;
            template.Cells["D51"].Value = data.XPG.CompanyMargin;

            template.Cells["E38"].Value = data.Total.ActiveMember;
            template.Cells["E39"].Value = data.Total.BetCount;
            template.Cells["E40"].Value = data.Total.MemberTO;
            template.Cells["E41"].Value = data.Total.NetTO;
            template.Cells["E42"].Value = data.Total.MemberWL;
            template.Cells["E43"].Value = data.Total.CompanyTO_Live;
            template.Cells["E44"].Value = data.Total.CompanyTO_NonLive;
            template.Cells["E45"].Value = data.Total.CompanyTO_Mix;
            template.Cells["E46"].Value = data.Total.CompanyTO;
            template.Cells["E47"].Value = data.Total.CompanyIncome_Live;
            template.Cells["E48"].Value = data.Total.CompanyIncome_NonLive;
            template.Cells["E49"].Value = data.Total.CompanyIncome_Mix;
            template.Cells["E50"].Value = data.Total.CompanyIncome;
            template.Cells["E51"].Value = data.Total.CompanyMargin;
        }

        private void RenderPeriodEN(ReportModel data)
        {
            template.Cells["B19"].Value = $"   [Current Period: {data.Date}]";
            template.Cells["C21"].Value = data.TPG.ActiveMember;
            template.Cells["C22"].Value = data.TPG.BetCount;
            template.Cells["C23"].Value = data.TPG.MemberTO;
            template.Cells["C24"].Value = data.TPG.NetTO;
            template.Cells["C25"].Value = data.TPG.MemberWL;
            template.Cells["C26"].Value = data.TPG.CompanyTO_Live;
            template.Cells["C27"].Value = data.TPG.CompanyTO_NonLive;
            template.Cells["C28"].Value = data.TPG.CompanyTO_Mix;
            template.Cells["C29"].Value = data.TPG.CompanyTO;
            template.Cells["C30"].Value = data.TPG.CompanyIncome_Live;
            template.Cells["C31"].Value = data.TPG.CompanyIncome_NonLive;
            template.Cells["C32"].Value = data.TPG.CompanyIncome_Mix;            
            template.Cells["C33"].Value = data.TPG.CompanyIncome;
            template.Cells["C34"].Value = data.TPG.CompanyMargin;

            template.Cells["D21"].Value = data.XPG.ActiveMember;
            template.Cells["D22"].Value = data.XPG.BetCount;
            template.Cells["D23"].Value = data.XPG.MemberTO;
            template.Cells["D24"].Value = data.XPG.NetTO;
            template.Cells["D25"].Value = data.XPG.MemberWL;
            template.Cells["D26"].Value = data.XPG.CompanyTO_Live;
            template.Cells["D27"].Value = data.XPG.CompanyTO_NonLive;
            template.Cells["D28"].Value = data.XPG.CompanyTO_Mix;
            template.Cells["D29"].Value = data.XPG.CompanyTO;
            template.Cells["D30"].Value = data.XPG.CompanyIncome_Live;
            template.Cells["D31"].Value = data.XPG.CompanyIncome_NonLive;
            template.Cells["D32"].Value = data.XPG.CompanyIncome_Mix;
            template.Cells["D33"].Value = data.XPG.CompanyIncome;
            template.Cells["D34"].Value = data.XPG.CompanyMargin;

            template.Cells["E21"].Value = data.Total.ActiveMember;
            template.Cells["E22"].Value = data.Total.BetCount;
            template.Cells["E23"].Value = data.Total.MemberTO;
            template.Cells["E24"].Value = data.Total.NetTO;
            template.Cells["E25"].Value = data.Total.MemberWL;
            template.Cells["E26"].Value = data.Total.CompanyTO_Live;
            template.Cells["E27"].Value = data.Total.CompanyTO_NonLive;
            template.Cells["E28"].Value = data.Total.CompanyTO_Mix;
            template.Cells["E29"].Value = data.Total.CompanyTO;
            template.Cells["E30"].Value = data.Total.CompanyIncome_Live;
            template.Cells["E31"].Value = data.Total.CompanyIncome_NonLive;
            template.Cells["E32"].Value = data.Total.CompanyIncome_Mix;
            template.Cells["E33"].Value = data.Total.CompanyIncome;
            template.Cells["E34"].Value = data.Total.CompanyMargin;
        }

        private void RenderPeriodCN(ReportModel data)
        {
            template.Cells["B53"].Value = $"   [本期: {data.Date}]";
            template.Cells["C55"].Value = data.TPG.ActiveMember;
            template.Cells["C56"].Value = data.TPG.BetCount;
            template.Cells["C57"].Value = data.TPG.MemberTO;
            template.Cells["C58"].Value = data.TPG.NetTO;
            template.Cells["C59"].Value = data.TPG.MemberWL;
            template.Cells["C60"].Value = data.TPG.CompanyTO_Live;
            template.Cells["C61"].Value = data.TPG.CompanyTO_NonLive;
            template.Cells["C62"].Value = data.TPG.CompanyTO_Mix;
            template.Cells["C63"].Value = data.TPG.CompanyTO;
            template.Cells["C64"].Value = data.TPG.CompanyIncome_Live;
            template.Cells["C65"].Value = data.TPG.CompanyIncome_NonLive;
            template.Cells["C66"].Value = data.TPG.CompanyIncome_Mix;
            template.Cells["C67"].Value = data.TPG.CompanyIncome;
            template.Cells["C68"].Value = data.TPG.CompanyMargin;

            template.Cells["D55"].Value = data.XPG.ActiveMember;
            template.Cells["D56"].Value = data.XPG.BetCount;
            template.Cells["D57"].Value = data.XPG.MemberTO;
            template.Cells["D58"].Value = data.XPG.NetTO;
            template.Cells["D59"].Value = data.XPG.MemberWL;
            template.Cells["D60"].Value = data.XPG.CompanyTO_Live;
            template.Cells["D61"].Value = data.XPG.CompanyTO_NonLive;
            template.Cells["D62"].Value = data.XPG.CompanyTO_Mix;
            template.Cells["D63"].Value = data.XPG.CompanyTO;
            template.Cells["D64"].Value = data.XPG.CompanyIncome_Live;
            template.Cells["D65"].Value = data.XPG.CompanyIncome_NonLive;
            template.Cells["D66"].Value = data.XPG.CompanyIncome_Mix;
            template.Cells["D67"].Value = data.XPG.CompanyIncome;
            template.Cells["D68"].Value = data.XPG.CompanyMargin;

            template.Cells["E55"].Value = data.Total.ActiveMember;
            template.Cells["E56"].Value = data.Total.BetCount;
            template.Cells["E57"].Value = data.Total.MemberTO;
            template.Cells["E58"].Value = data.Total.NetTO;
            template.Cells["E59"].Value = data.Total.MemberWL;
            template.Cells["E60"].Value = data.Total.CompanyTO_Live;
            template.Cells["E61"].Value = data.Total.CompanyTO_NonLive;
            template.Cells["E62"].Value = data.Total.CompanyTO_Mix;
            template.Cells["E63"].Value = data.Total.CompanyTO;
            template.Cells["E64"].Value = data.Total.CompanyIncome_Live;
            template.Cells["E65"].Value = data.Total.CompanyIncome_NonLive;
            template.Cells["E66"].Value = data.Total.CompanyIncome_Mix;
            template.Cells["E67"].Value = data.Total.CompanyIncome;
            template.Cells["E68"].Value = data.Total.CompanyMargin;
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