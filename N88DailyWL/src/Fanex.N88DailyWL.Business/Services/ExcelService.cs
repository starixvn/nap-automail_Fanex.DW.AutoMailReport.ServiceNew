using Fanex.N88DailyWL.Business.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface ExcelService
    {
        string ExportCurrency(List<N88CurrencyModel> data, bool isYesterday);

        string ExportProduct(List<N88ProductModel> data, bool isYesterday);
    }

    public class ExcelServiceImp : ExcelService
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;
        private ExcelWorksheet sheetCurrency;
        private ExcelWorksheet sheetCurrencyTotal;
        private ExcelWorksheet sheetProduct;
        private ExcelWorksheet sheetProductTotal;

        public ExcelServiceImp(ExcelPackage excelPackage, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;
        }

        public string ExportCurrency(List<N88CurrencyModel> data, bool isYesterday)
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/N88Template.xlsx"));
            sheetCurrency = excelPackage.Workbook.Worksheets["Currency"];
            sheetCurrencyTotal = excelPackage.Workbook.Worksheets["Currency-Total"];

            if (data.Count > 0)
            {
                data.Add(new N88CurrencyModel
                {
                    Currency = "Grand Total",
                    ActiveMembers = data.Sum(s => s.ActiveMembers),
                    DepositMembers = data.Sum(s => s.DepositMembers),
                    FirstDepositTrans = data.Sum(s => s.FirstDepositTrans),
                    DepositTrans = data.Sum(s => s.DepositTrans)
                });

                RenderBody_Currency(data, isYesterday);
            }
            else
            {
                sheetCurrency.Cells[4, 2, 4, 6].Merge = true;
                sheetCurrency.Cells[4, 2, 4, 6].Value = "There is no data.";
                sheetCurrency.Cells[4, 2, 4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            return SaveFile_Currency(isYesterday);
        }

        private void RenderBody_Currency(List<N88CurrencyModel> data, bool isYesterday)
        {
            sheetCurrency.Cells["B2"].Value = isYesterday ? $"{dateService.ToDate:yyyyMMdd}" : "MTD";
            sheetCurrency.Cells["B4"].LoadFromCollection(data, false);
            var maxRowIndex = 3 + data.Count;
            for (int columnIndex = 2; columnIndex < 7; columnIndex++)
            {
                sheetCurrency.Cells[4, columnIndex, maxRowIndex, columnIndex].StyleID = sheetCurrency.Cells[4, columnIndex].StyleID;
            }

            for (int columnIndex = 2; columnIndex < 7; columnIndex++)
            {
                sheetCurrency.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = sheetCurrencyTotal.Cells[2, columnIndex].StyleID;
            }
        }

        private string SaveFile_Currency(bool isYesterday)
        {
            excelPackage.Workbook.Worksheets.Delete("Product");
            excelPackage.Workbook.Worksheets.Delete("Currency-Total");
            excelPackage.Workbook.Worksheets.Delete("Product-Total");
            var yesterday = dateService.ToDate;
            var fileName = isYesterday
                ? $"N88-Currency-{yesterday:yyyyMMdd}.xlsx"
                : $"N88-Currency-MTD-{yesterday:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{yesterday:yyyy}\{yesterday:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }

        private string SaveFile_Product(bool isYesterday)
        {
            excelPackage.Workbook.Worksheets.Delete("Currency");
            excelPackage.Workbook.Worksheets.Delete("Currency-Total");
            excelPackage.Workbook.Worksheets.Delete("Product-Total");
            var yesterday = dateService.ToDate;
            var fileName = isYesterday
                ? $"N88-Product-{yesterday:yyyyMMdd}.xlsx"
                : $"N88-Product-MTD-{yesterday:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{yesterday:yyyy}\{yesterday:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }

        public string ExportProduct(List<N88ProductModel> data, bool isYesterday)
        {
            excelPackage = new ExcelPackage(new FileInfo("Templates/N88Template.xlsx"));
            sheetProduct = excelPackage.Workbook.Worksheets["Product"];
            sheetProductTotal = excelPackage.Workbook.Worksheets["Product-Total"];

            var coIncome = data.Sum(s => s.CoIncome);
            var coTO = data.Sum(s => s.CoTurnover);
            var coMargin = coTO == 0 ? 0 : coIncome / coTO;

            data.Add(new N88ProductModel
            {
                Product = "Grand Total",
                BetCount = data.Sum(s => s.BetCount),
                Turnover = data.Sum(s => s.Turnover),
                Winloss = data.Sum(s => s.Winloss),
                CoTurnover = data.Sum(s => s.CoTurnover),
                CoIncome = data.Sum(s => s.CoIncome),
                CoMargin = coMargin
            });

            RenderBody_Product(data, isYesterday);
            return SaveFile_Product(isYesterday);
        }

        private void RenderBody_Product(List<N88ProductModel> data, bool isYesterday)
        {
            sheetProduct.Cells["B2"].Value = isYesterday ? $"{dateService.ToDate:yyyyMMdd}" : "MTD";
            sheetProduct.Cells["B4"].LoadFromCollection(data, false);
            var maxRowIndex = 3 + data.Count;
            for (int columnIndex = 2; columnIndex < 9; columnIndex++)
            {
                sheetProduct.Cells[4, columnIndex, maxRowIndex, columnIndex].StyleID = sheetProduct.Cells[4, columnIndex].StyleID;
            }

            for (int rowIndex = 4; rowIndex <= maxRowIndex; rowIndex++)
            {
                var isLiveText = sheetProduct.Cells[rowIndex, 2].Value.ToString().Contains("Live");
                var isBold = !isLiveText;
                var isRightAlign = isLiveText;
                var isBlueBGColor = !isLiveText;
                var isGrandTotalRow = sheetProduct.Cells[rowIndex, 2].Value.ToString().Contains("Grand");

                sheetProduct.Row(rowIndex).Style.Font.Bold = isBold;
                sheetProduct.Cells[rowIndex, 2].Style.HorizontalAlignment = isRightAlign ? ExcelHorizontalAlignment.Right : ExcelHorizontalAlignment.Left;
                sheetProduct.Row(rowIndex).Height = 19.5;

                if (isBlueBGColor)
                {
                    for (int columnIndex = 2; columnIndex < 9; columnIndex++)
                    {
                        sheetProduct.Cells[rowIndex, columnIndex, rowIndex, columnIndex].StyleID = sheetProductTotal.Cells[4, columnIndex].StyleID;
                    }
                }

                if (isGrandTotalRow)
                {
                    for (int columnIndex = 2; columnIndex < 9; columnIndex++)
                    {
                        sheetProduct.Cells[maxRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = sheetProductTotal.Cells[2, columnIndex].StyleID;
                    }
                }
            }
        }
    }
}