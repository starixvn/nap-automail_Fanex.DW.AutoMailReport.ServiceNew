using System;
using System.Collections.Generic;
using System.IO;
using BetradarVirtualSports.Business.Models;
using Epplus.Mapper.Extensions;
using OfficeOpenXml;

namespace BetradarVirtualSports.Business.Services
{
    public interface ExcelService
    {
        ExcelService Export();

        string Save();
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet betradarVS_Daiily;

        public ExcelService Export()
        {
            var templateAbsolutelyPath = @"Templates\Template.xlsx";
            using (var stream = new FileStream(templateAbsolutelyPath, FileMode.Open, FileAccess.Read))
            {
                excelPackage = new ExcelPackage(stream);
                betradarVS_Daiily = excelPackage.Workbook.Worksheets["BetradarVS_Daiily"];
            }

            var data = new List<GrossWinningDaily>
            {
                new GrossWinningDaily
                {
                    Date = new DateTime(2017, 01, 01),
                    Business = "Cash",
                    Currency = "MYR",
                    BetCount = 1000,
                    TotalBet = 1200,
                    TotalPayout = 1300,
                    TotalWinLoss = 4520,
                    Margin = 1.86m
                },
                new GrossWinningDaily
                {
                    Date = new DateTime(2017, 01, 01),
                    Business = "Cash",
                    Currency = "MYR",
                    BetCount = 1200,
                    TotalBet = 1300,
                    TotalPayout = 1100,
                    TotalWinLoss = 400,
                    Margin = 7.96m
                },
            };

            betradarVS_Daiily.ApplyVertical(data);

            return this;
        }

        public string Save()
        {
            var filename = @"D:\Fanex.Excel\BetradarVS_Daiily.xlsx";
            excelPackage.SaveAs(new FileInfo(filename), string.Empty);
            return string.Empty;
        }
    }
}