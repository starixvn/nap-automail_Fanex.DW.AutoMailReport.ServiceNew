using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DataMarginService
    {
        List<MarginModel> CalcDataForDaily(DataTable source);

        List<MarginModel> CalcDataForMTD(DataTable source);
    }

    public class DataMarginServiceImp : DataMarginService
    {
        private readonly BaseDataTable baseDataTable;

        public DataMarginServiceImp(BaseDataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
        }

        public List<MarginModel> CalcDataForDaily(DataTable source)
        {
            var data = baseDataTable.CloneWithoutIgnoreFigureRows(source);
            var result = new List<MarginModel>();
            var dailyColumnIndex = data.Columns.Count - 1;

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
                    var margrin = new MarginModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        Seven89YTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),
                        Seven89YWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),

                        MBCTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),
                        MBCWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),

                        LicenseeTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && !Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),
                        LicenseeWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && !Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),

                        TotalTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),
                        TotalWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[dailyColumnIndex])),
                    };

                    result.Add(margrin);
                });

            CalcGrandTotal(result);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        public List<MarginModel> CalcDataForMTD(DataTable source)
        {
            var data = baseDataTable.CloneWithoutIgnoreFigureRows(source);
            var result = new List<MarginModel>();
            var mtdColumnIndex = 9;

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
                    var margrin = new MarginModel
                    {
                        ProductShortName = product.Key.ProductShortName,
                        ProductGroup = product.Key.ProductGroup,
                        OrderByProduct = product.Key.OrderByProduct,

                        Seven89YTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),
                        Seven89YWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.Seven89Y))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),

                        MBCTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),
                        MBCWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && s[ColumnName.SiteGroup].Equals(Site.MBC))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),

                        LicenseeTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && !Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),
                        LicenseeWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver) && !Converter.To<bool>(s[ColumnName.IsDeposit]))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),

                        TotalTO = product.Where(s => s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),
                        TotalWL = product.Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                        .Sum(s => Converter.To<decimal>(s[mtdColumnIndex])),
                    };

                    result.Add(margrin);
                });

            CalcGrandTotal(result);

            return result.OrderBy(o => o.OrderByProduct).ToList();
        }

        private void CalcGrandTotal(List<MarginModel> data)
        {
            var grandTotal = new MarginModel
            {
                ProductShortName = "Total",
                Seven89YTO = data.Sum(s => s.Seven89YTO),
                Seven89YWL = data.Sum(s => s.Seven89YWL),
                MBCTO = data.Sum(s => s.MBCTO),
                MBCWL = data.Sum(s => s.MBCWL),
                LicenseeTO = data.Sum(s => s.LicenseeTO),
                LicenseeWL = data.Sum(s => s.LicenseeWL),
                TotalTO = data.Sum(s => s.TotalTO),
                TotalWL = data.Sum(s => s.TotalWL),
                ProductGroup = "Total",
                OrderByProduct = 100000
            };

            data.Add(grandTotal);
        }
    }
}