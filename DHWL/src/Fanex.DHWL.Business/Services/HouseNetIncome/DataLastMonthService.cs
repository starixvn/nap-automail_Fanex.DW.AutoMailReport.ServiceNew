using Fanex.Common;
using Fanex.DHWL.Business.Models;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.HouseNetIncome
{
    public interface DataLastMonthService
    {
        DataTable CalcData(DataTable source);
    }

    public class DataLastMonthServiceImp : DataLastMonthService
    {
        public DataTable CalcData(DataTable source)
        {
            var productData = new DataTable();
            productData.Columns.Add(ColumnName.ProductName, typeof(string));
            productData.Columns.Add(ColumnName.MTD, typeof(decimal));

            source.AsEnumerable()
                .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                .ToList()
                .ForEach(row => { row[ColumnName.OrderByProduct] = 1; });

            source.AsEnumerable().GroupBy(product => new
            {
                ProductName = product[ColumnName.ProductName],
                OrderByProduct = Converter.To<int>(product[ColumnName.OrderByProduct])
            })
                .OrderBy(o => o.Key.OrderByProduct)
                .ToList()
                .ForEach(product =>
                {
                    var productRow = productData.NewRow();
                    productRow[ColumnName.ProductName] = product.Key.ProductName;
                    productRow[ColumnName.MTD] = product.Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
                    productData.Rows.Add(productRow);
                });

            var grandTotalRow = productData.NewRow();
            var mtdGrandTotal = source.AsEnumerable().Sum(s => Converter.To<decimal>(s[ColumnName.MTD]));
            grandTotalRow[ColumnName.ProductName] = "Grand Total";
            grandTotalRow[ColumnName.MTD] = mtdGrandTotal;
            productData.Rows.Add(grandTotalRow);

            var netIncome = source.AsEnumerable().Sum(s => Converter.To<decimal>(s[ColumnName.Monthly]));
            var adjustment = netIncome - mtdGrandTotal;

            var adjustmentRow = productData.NewRow();
            adjustmentRow[ColumnName.ProductName] = "Adjustment";
            adjustmentRow[ColumnName.MTD] = adjustment;
            productData.Rows.Add(adjustmentRow);

            var netIncomeRow = productData.NewRow();
            netIncomeRow[ColumnName.ProductName] = "Net Income";
            netIncomeRow[ColumnName.MTD] = netIncome;
            productData.Rows.Add(netIncomeRow);

            return productData;
        }
    }
}