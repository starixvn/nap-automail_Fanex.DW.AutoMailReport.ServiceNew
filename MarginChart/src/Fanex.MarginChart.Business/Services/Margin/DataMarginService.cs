using Fanex.Common;
using Fanex.MarginChart.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.MarginChart.Business.Services.Margin
{
    public interface DataMarginService
    {
        List<MarginModel> CalcDataByCategory(DataTable source, string category, int columnIndex);

        Dictionary<int, double> CalcTurnoverForChart(string category, DataTable data, DataType dataType);

        Dictionary<int, double> CalcWinLossForChart(string category, DataTable data, DataType dataType);
    }

    public class DataMarginServiceImp : DataMarginService
    {
        public List<MarginModel> CalcDataByCategory(DataTable source, string category, int columnIndex)
        {
            var result = new List<MarginModel>();
            source.AsEnumerable()
            .Where(s => s[ColumnName.ProductCategory].Equals(category))
            .GroupBy(g => new { isDeposit = Converter.To<bool>(g[ColumnName.IsDeposit]) })
            .ToList()
            .ForEach(site =>
            {
                var model = new MarginModel
                {
                    Site = site.Key.isDeposit ? "Licensee" : "Credit",
                    HWL = site.Where(s => !s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)).Sum(s => Converter.To<decimal>(s[columnIndex])),
                    TO = site.Where(s => s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)).Sum(s => Converter.To<decimal>(s[columnIndex])),
                };
                result.Add(model);
            });

            var total = new MarginModel
            {
                Site = "Total",
                HWL = result.Sum(s => s.HWL),
                TO = result.Sum(s => s.TO)
            };

            result.Add(total);

            return result;
        }

        public Dictionary<int, double> CalcTurnoverForChart(string category, DataTable data, DataType dataType)
        {
            var result = new Dictionary<int, double>();
            var dataSource = data.Rows.Count > 0
                ? RefineCalcTO(category, data, dataType)
                : data;

            for (var columnIndex = ColumnIndex.StartDateColumnIndex; columnIndex < dataSource.Columns.Count; columnIndex++)
            {
                var to = dataSource.AsEnumerable()
                    .Where(s => s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => Converter.To<double>(s[columnIndex]));
                var date = Converter.To<int>(dataSource.Columns[columnIndex].ColumnName);
                result.Add(date, to);
            }

            return result;
        }

        private static DataTable RefineCalcTO(string category, DataTable data, DataType dataType)
        {
            return (dataType == DataType.All
                            ? data.AsEnumerable().Where(s => s[ColumnName.ProductCategory].Equals(category)
                            && s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                            : data.AsEnumerable().Where(s => s[ColumnName.ProductCategory].Equals(category)
                            && s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)
                            && Converter.To<bool>(s[ColumnName.IsDeposit]) == Converter.To<bool>(dataType)).CopyToDataTable());
        }

        public Dictionary<int, double> CalcWinLossForChart(string category, DataTable data, DataType dataType)
        {
            var result = new Dictionary<int, double>();
            var dataSource = data.Rows.Count > 0
                ? RefineWinLoss(category, data, dataType)
                : data;

            for (var columnIndex = ColumnIndex.StartDateColumnIndex; columnIndex < dataSource.Columns.Count; columnIndex++)
            {
                var winloss = dataSource.AsEnumerable()
                    .Where(s => !s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver))
                    .Sum(s => Converter.To<double>(s[columnIndex]));
                var date = Converter.To<int>(dataSource.Columns[columnIndex].ColumnName);
                result.Add(date, winloss);
            }

            return result;
        }

        private static DataTable RefineWinLoss(string category, DataTable data, DataType dataType)
        {
            return (dataType == DataType.All
                            ? data.AsEnumerable().Where(s => s[ColumnName.ProductCategory].Equals(category)
                            && !s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)).CopyToDataTable()
                            : data.AsEnumerable().Where(s => s[ColumnName.ProductCategory].Equals(category)
                            && !s[ColumnName.Figure].Equals(RowFigureName.HouseTurnOver)
                            && Converter.To<bool>(s[ColumnName.IsDeposit]) == Converter.To<bool>(dataType)).CopyToDataTable());
        }
    }
}