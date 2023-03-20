using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DataMarginService
    {
        List<MarginModel> CalcDataForDaily(DataTable source);

        List<MarginModel> CalcDataForMtd(DataTable source);

        List<MarginModel> GrandTotalList { get; set; }
    }

    public class DataMarginServiceImp : DataMarginService
    {
        private readonly BaseDataTable baseDataTable;

        public DataMarginServiceImp(BaseDataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
        }

        public List<MarginModel> GrandTotalList { get; set; }

        public List<MarginModel> CalcDataForDaily(DataTable source)
        {
            var data = baseDataTable.CloneWithoutIgnoreFigureRows(source);
            var royalFeesData = baseDataTable.CloneRoyaltyFees(source);
            var result = new List<MarginModel>();
            var dailyColumnIndex = data.Columns.Count - 1;
            CalcData(data, dailyColumnIndex, result);
            var royalFeesResult = CalcRoyalFeesData(royalFeesData, dailyColumnIndex);
            CalcGrandTotal(result, royalFeesResult);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        public List<MarginModel> CalcDataForMtd(DataTable source)
        {
            var data = baseDataTable.CloneWithoutIgnoreFigureRows(source);
            var royalFeesData = baseDataTable.CloneRoyaltyFees(source);
            var result = new List<MarginModel>();
            var mtdColumnIndex = ColumnIndex.MTDColumnIndex;
            CalcData(data, mtdColumnIndex, result);
            var royalFeesResult = CalcRoyalFeesData(royalFeesData, mtdColumnIndex);
            CalcGrandTotal(result, royalFeesResult);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private static void CalcData(DataTable data, int calcAtColumnIndex, List<MarginModel> result)
        {
            data.AsEnumerable()
                .GroupBy(g => new
                {
                    ProductShortName = g[ColumnName.ProductShortName].ToString(),
                    ProductGroup = g[ColumnName.ProductGroup].ToString(),
                    OrderByProduct = Converter.To<int>(g[ColumnName.OrderByProduct])
                })
                .ToList()
                .ForEach(product =>
                {
                    var margin = new MarginModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        Seven89YTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                        Seven89YWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),

                        MBCTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                        MBCWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),

                        LicenseeTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                        LicenseeWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),

                        TotalTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                        TotalWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                    };

                    result.Add(margin);
                });
        }

        private static MarginModel CalcRoyalFeesData(DataTable data, int calcAtColumnIndex)
        {
            var royaltyFees = new MarginModel
            {
                ProductShortName = $"Royalty Fees{Environment.NewLine}To 3rd Parties",
                ProductGroup = "Total",
                OrderByProduct = 100001,

                Seven89YWL = data.AsEnumerable().Where(s => s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                         .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                MBCWL = data.AsEnumerable().Where(s => s[ColumnName.SiteGroup].Equals(Site.MBC))
                         .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                LicenseeWL = data.AsEnumerable().Where(s => Converter.To<bool>(s[ColumnName.IsDeposit]))
                         .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                TotalWL = data.AsEnumerable()
                         .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
            };

            return royaltyFees;
        }

        private void CalcGrandTotal(List<MarginModel> data, MarginModel royalFeesResult)
        {
            GrandTotalList = new List<MarginModel>();
            var grandTotal = new MarginModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 100000,
                Seven89YTO = data.Sum(s => s.Seven89YTO),
                Seven89YWL = data.Sum(s => s.Seven89YWL),
                MBCTO = data.Sum(s => s.MBCTO),
                MBCWL = data.Sum(s => s.MBCWL),
                LicenseeTO = data.Sum(s => s.LicenseeTO),
                LicenseeWL = data.Sum(s => s.LicenseeWL),
                TotalTO = data.Sum(s => s.TotalTO),
                TotalWL = data.Sum(s => s.TotalWL)
            };

            var netTotal = new MarginModel
            {
                ProductShortName = "NET TOTAL",
                ProductGroup = "Total",
                OrderByProduct = 100002,
                Seven89YWL = grandTotal.Seven89YWL + royalFeesResult.Seven89YWL,
                MBCWL = grandTotal.MBCWL + royalFeesResult.MBCWL,
                LicenseeWL = grandTotal.LicenseeWL + royalFeesResult.LicenseeWL,
                TotalWL = grandTotal.TotalWL + royalFeesResult.TotalWL,
                TotalTO = grandTotal.TotalTO,
                LicenseeTO = grandTotal.LicenseeTO,
            };

            data.Add(grandTotal);
            data.Add(royalFeesResult);
            data.Add(netTotal);

            GrandTotalList.Add(grandTotal);
            GrandTotalList.Add(netTotal);
        }
    }
}