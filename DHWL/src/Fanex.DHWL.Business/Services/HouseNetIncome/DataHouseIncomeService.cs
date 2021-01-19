using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DataHouseIncomeService
    {
        DataTable CalTotalByProductForExcel(DataTable source);
    }

    public class DataHouseIncomeServiceImp : DataHouseIncomeService
    {
        private readonly BaseDataTable baseDataTable;

        public DataHouseIncomeServiceImp(BaseDataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
        }

        public DataTable CalTotalByProductForExcel(DataTable source)
        {
            var data = baseDataTable.Clone(source);
            var columns = new List<string>
            {
                ColumnName.IsDeposit,
                ColumnName.SiteGroup,
                ColumnName.ProductCategory,
                ColumnName.ProductGroup,
                ColumnName.ProductShortName,
                ColumnName.Monthly
            };

            columns.ForEach(column =>
            {
                data.Columns.Remove(column);
            });

            var result1 = data.Clone();
            var result2 = data.Clone();

            data.AsEnumerable().GroupBy(row => new
            {
                ProductName = row[ColumnName.ProductName],
                Figure = row[ColumnName.Figure]
            })
            .Where(t => !t.Key.Figure.Equals(RowFigureName.HouseTurnOver)).ToList()
            .ForEach(product =>
            {
                CalcTotal(result1, product.ToList(), product.Key.ProductName.ToString(), product.Key.Figure.ToString());
            });

            result1.AsEnumerable().GroupBy(row => new
            {
                ProductName = row[ColumnName.ProductName]
            }).ToList()
            .ForEach(product =>
            {
                foreach (DataRow r in product)
                {
                    result2.ImportRow(r);
                }

                CalcTotal(result2, product.Key.ProductName.ToString(), "Total");
            });

            var rows = data.AsEnumerable().Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver)).ToList();
            CalcTotal(result2, rows, "Grand Total", string.Empty);
            result2 = ReOrder(data, result2);

            return result2;
        }

        private static void CalcTotal(DataTable dataTable, string productName, string figure)
        {
            var totalRow = dataTable.NewRow();
            totalRow[ColumnName.ProductName] = productName;
            totalRow[ColumnName.Figure] = figure;
            for (int columIndex = 4; columIndex < dataTable.Columns.Count; columIndex++)
            {
                totalRow[columIndex] = dataTable.AsEnumerable().Where(p => p[ColumnName.ProductName].ToString() == productName)
                    .Sum(s => Converter.To<decimal>(s[columIndex]));
            }

            dataTable.Rows.Add(totalRow);
        }

        private static void CalcTotal(DataTable dataTable, List<DataRow> source, string productName, string figure)
        {
            var totalRow = dataTable.NewRow();
            totalRow[ColumnName.ProductName] = productName;
            totalRow[ColumnName.Figure] = figure;
            for (int columIndex = 4; columIndex < dataTable.Columns.Count; columIndex++)
            {
                totalRow[columIndex] = source.Sum(s => Converter.To<decimal>(s[columIndex]));
            }

            dataTable.Rows.Add(totalRow);
        }

        private static DataTable ReOrder(DataTable data, DataTable result2)
        {
            result2.AsEnumerable().ToList()
            .ForEach(row =>
            {
                var sourceRow = data.AsEnumerable().FirstOrDefault(s => s[ColumnName.ProductName] == row[ColumnName.ProductName]);
                if (sourceRow == null)
                {
                    row[ColumnName.OrderByProduct] = 1000;
                    row[ColumnName.OrderByFigure] = 1000;
                }
                else
                {
                    var orderByProduct = row[ColumnName.ProductName].ToString() == "Sportsbook" ? 1 : sourceRow[ColumnName.OrderByProduct];
                    var orderByFigure = row[ColumnName.Figure].ToString() == "Total" ? 100 : sourceRow[ColumnName.OrderByFigure];
                    row[ColumnName.OrderByProduct] = orderByProduct;
                    row[ColumnName.OrderByFigure] = orderByFigure;
                }
            });

            result2 = result2.AsEnumerable()
                .OrderBy(s => s[ColumnName.OrderByProduct])
                .ThenBy(s => s[ColumnName.OrderByFigure])
                .CopyToDataTable();

            var columns = new List<string>
            {
                ColumnName.OrderByProduct,
                ColumnName.OrderByFigure
            };

            columns.ForEach(column =>
            {
                result2.Columns.Remove(column);
            });

            return result2;
        }
    }
}