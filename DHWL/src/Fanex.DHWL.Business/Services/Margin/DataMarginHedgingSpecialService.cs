using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DataMarginHedgingSpecialService
    {
        List<MarginHedgingSpecialModel> CalcDataForDaily(DataTable source);

        List<MarginHedgingSpecialModel> CalcDataForMtd(DataTable source);

        List<MarginHedgingSpecialModel> GrandTotalList { get; set; }
    }

    public class DataMarginHedgingSpecialServiceImp : DataMarginHedgingSpecialService
    {
        private readonly BaseDataTable baseDataTable;

        public DataMarginHedgingSpecialServiceImp(BaseDataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
        }

        public List<MarginHedgingSpecialModel> GrandTotalList { get; set; }

        public List<MarginHedgingSpecialModel> CalcDataForDaily(DataTable source)
        {
            var data = source.Rows.Count > 0
                ? baseDataTable.CloneWithoutIgnoreFigureRows(source)
                    .AsEnumerable()
                    .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                    .CopyToDataTable()
                : source;
            var royalFeesData = source.Rows.Count > 0
                ? baseDataTable.CloneRoyaltyFees(source)
                    .AsEnumerable()
                    .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                    .CopyToDataTable()
                : source;

            var result = new List<MarginHedgingSpecialModel>();
            var dailyColumnIndex = data.Columns.Count - 1;
            CalcData(data, dailyColumnIndex, result);
            var royalFeesResult = CalcRoyalFeesData(royalFeesData, dailyColumnIndex);
            CalcGrandTotal(result, royalFeesResult);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        public List<MarginHedgingSpecialModel> CalcDataForMtd(DataTable source)
        {
            var data = source.Rows.Count > 0
                ? baseDataTable.CloneWithoutIgnoreFigureRows(source)
                    .AsEnumerable()
                    .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                    .CopyToDataTable()
                : source;
            var royalFeesData = source.Rows.Count > 0
                ? baseDataTable.CloneRoyaltyFees(source)
                    .AsEnumerable()
                    .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                    .CopyToDataTable()
                : source;
            var result = new List<MarginHedgingSpecialModel>();
            var mtdColumnIndex = ColumnIndex.MTDColumnIndex;
            CalcData(data, mtdColumnIndex, result);
            var royalFeesResult = CalcRoyalFeesData(royalFeesData, mtdColumnIndex);
            CalcGrandTotal(result, royalFeesResult);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private static void CalcData(DataTable data, int calcAtColumnIndex, List<MarginHedgingSpecialModel> result)
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
                    var hedgingTo = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Hedging))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex]));
                    var hedgingWl = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Hedging))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex]));
                    var specialTo = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Special))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex]));
                    var specialWl = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Special))
                        .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex]));

                    var margin = new MarginHedgingSpecialModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        Seven89YTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                            .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                        Seven89YWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                            .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),

                        HedgingTO = hedgingTo,
                        HedgingWL = hedgingWl,

                        SpecialTO = specialTo,
                        SpecialWL = specialWl,

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

        private static MarginHedgingSpecialModel CalcRoyalFeesData(DataTable data, int calcAtColumnIndex)
        {
            var royaltyFees = new MarginHedgingSpecialModel
            {
                ProductShortName = $"Royalty Fees{Environment.NewLine}To 3rd Parties",
                ProductGroup = "Total",
                OrderByProduct = 100001,

                Seven89YWL = data.AsEnumerable().Where(s => s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                         .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                HedgingWL = data.AsEnumerable().Where(s => s[ColumnName.SiteGroup].Equals(Site.Hedging))
                    .Sum(s => Converter.To<decimal>(s[calcAtColumnIndex])),
                SpecialWL = data.AsEnumerable().Where(s => s[ColumnName.SiteGroup].Equals(Site.Special))
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

        private void CalcGrandTotal(List<MarginHedgingSpecialModel> data, MarginHedgingSpecialModel royalFeesResult)
        {
            GrandTotalList = new List<MarginHedgingSpecialModel>();
            var grandTotal = new MarginHedgingSpecialModel
            {
                ProductShortName = "Total",
                ProductGroup = "Total",
                OrderByProduct = 100000,
                Seven89YTO = data.Sum(s => s.Seven89YTO),
                Seven89YWL = data.Sum(s => s.Seven89YWL),
                HedgingTO = data.Sum(s => s.HedgingTO),
                HedgingWL = data.Sum(s => s.HedgingWL),
                SpecialTO = data.Sum(s => s.SpecialTO),
                SpecialWL = data.Sum(s => s.SpecialWL),
                MBCTO = data.Sum(s => s.MBCTO),
                MBCWL = data.Sum(s => s.MBCWL),
                LicenseeTO = data.Sum(s => s.LicenseeTO),
                LicenseeWL = data.Sum(s => s.LicenseeWL),
                TotalTO = data.Sum(s => s.TotalTO),
                TotalWL = data.Sum(s => s.TotalWL)
            };

            var netTotal = new MarginHedgingSpecialModel
            {
                ProductShortName = "NET TOTAL",
                ProductGroup = "Total",
                OrderByProduct = 100002,
                Seven89YWL = grandTotal.Seven89YWL + royalFeesResult.Seven89YWL,
                HedgingWL = grandTotal.HedgingWL + royalFeesResult.HedgingWL,
                SpecialWL = grandTotal.SpecialWL + royalFeesResult.SpecialWL,
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