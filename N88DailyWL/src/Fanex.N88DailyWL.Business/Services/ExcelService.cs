using Fanex.N88DailyWL.Business.Models;
using OfficeOpenXml;
using System;
using System.IO;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface ExcelService
    {
        string ExportExcel(Tuple<N88DailyWLModel, N88DailyWLModel, DepositModel> data);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
        }

        public string ExportExcel(Tuple<N88DailyWLModel, N88DailyWLModel, DepositModel> data)
        {
            var n88Yesterday = data.Item1;
            var n88MTD = data.Item2;
            var n88Deposit = data.Item3;
            var template = excelPackage.Workbook.Worksheets["Template"];

            AssignN88Yesterday(n88Yesterday, template);
            AssignN88MTD(n88MTD, template);
            AssignDeposit(n88Deposit, template);

            return SaveFile();
        }

        private void AssignN88Yesterday(N88DailyWLModel n88Yesterday, ExcelWorksheet template)
        {
            template.Cells["C3"].Value = $"{dateService.ToDate:yyyyMMdd}";
            template.Cells["C4"].Value = n88Yesterday.ActiveMember;
            template.Cells["C5"].Value = n88Yesterday.DepositMember;
            template.Cells["C6"].Value = n88Yesterday.BetCount;
            template.Cells["C7"].Value = n88Yesterday.MemberTO;
            template.Cells["C8"].Value = n88Yesterday.MemberWL;

            template.Cells["C9"].Value = n88Yesterday.CompanyTO_Live;
            template.Cells["C10"].Value = n88Yesterday.CompanyTO_NonLive;
            template.Cells["C11"].Value = n88Yesterday.CompanyTO_Mix;
            template.Cells["C12"].Value = n88Yesterday.CompanyTO;
            template.Cells["C13"].Value = n88Yesterday.CompanyIncome_Live;
            template.Cells["C14"].Value = n88Yesterday.CompanyIncome_NonLive;
            template.Cells["C15"].Value = n88Yesterday.CompanyIncome_Mix;
            template.Cells["C16"].Value = n88Yesterday.CompanyIncome;
            template.Cells["C17"].Value = n88Yesterday.CompanyMargin;
        }

        private void AssignN88MTD(N88DailyWLModel n88MTD, ExcelWorksheet template)
        {
            template.Cells["D4"].Value = n88MTD.ActiveMember;
            template.Cells["D5"].Value = n88MTD.DepositMember;
            template.Cells["D6"].Value = n88MTD.BetCount;
            template.Cells["D7"].Value = n88MTD.MemberTO;
            template.Cells["D8"].Value = n88MTD.MemberWL;

            template.Cells["D9"].Value = n88MTD.CompanyTO_Live;
            template.Cells["D10"].Value = n88MTD.CompanyTO_NonLive;
            template.Cells["D11"].Value = n88MTD.CompanyTO_Mix;
            template.Cells["D12"].Value = n88MTD.CompanyTO;
            template.Cells["D13"].Value = n88MTD.CompanyIncome_Live;
            template.Cells["D14"].Value = n88MTD.CompanyIncome_NonLive;
            template.Cells["D15"].Value = n88MTD.CompanyIncome_Mix;
            template.Cells["D16"].Value = n88MTD.CompanyIncome;
            template.Cells["D17"].Value = n88MTD.CompanyMargin;
        }

        private void AssignDeposit(DepositModel n88Deposit, ExcelWorksheet template)
        {
            template.Cells["G3"].Value = $"{dateService.ToDate:yyyyMMdd}";

            template.Cells["H4"].Value = n88Deposit.FirstDepositMember_MYR;
            template.Cells["I4"].Value = n88Deposit.FirstDepositMember_THB;
            template.Cells["J4"].Value = n88Deposit.FirstDepositMember_IDR;
            template.Cells["K4"].Value = n88Deposit.FirstDepositMember_CNY;

            template.Cells["H5"].Value = n88Deposit.DepositMember_MYR;
            template.Cells["I5"].Value = n88Deposit.DepositMember_THB;
            template.Cells["J5"].Value = n88Deposit.DepositMember_IDR;
            template.Cells["K5"].Value = n88Deposit.DepositMember_CNY;

            template.Cells["H6"].Value = n88Deposit.Deposit_MYR;
            template.Cells["I6"].Value = n88Deposit.Deposit_THB;
            template.Cells["J6"].Value = n88Deposit.Deposit_IDR;
            template.Cells["K6"].Value = n88Deposit.Deposit_CNY;
        }

        private string SaveFile()
        {
            var yesterday = dateService.ToDate;
            var fileName = $"N88_DailyWL_For_{yesterday:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{yesterday:yyyy}\{yesterday:MM}";
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