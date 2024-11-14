using Fanex.MarginChart.Business.Models;
using System.Data;
using System.Linq;

namespace Fanex.MarginChart.Business.Services
{
    public interface BaseDataTable
    {
        DataTable Clone(DataTable source);

        DataTable CloneWithoutIgnoreFigureRows(DataTable source);

        DataTable CloneRoyaltyFees(DataTable source);

        DataTable CombineAllInOneTable(DataSet dataSet);
    }

    public class BaseDataTableImp : BaseDataTable
    {
        private readonly RowFigureName rowFigureName = new RowFigureName();

        public DataTable Clone(DataTable source)
        {
            var result = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                result.ImportRow(row);
            }

            return result;
        }

        public DataTable CloneRoyaltyFees(DataTable source)
        {
            var result = source.Clone();
            var subSource = source
                .AsEnumerable()
                .Where(s => rowFigureName.RoyaltyFees.Contains(s["Figure"]))
                .ToList();
            subSource.ForEach(row =>
            {
                result.ImportRow(row);
            });

            return result;
        }

        public DataTable CloneWithoutIgnoreFigureRows(DataTable source)
        {
            var result = source.Clone();
            var subSource = source
                .AsEnumerable()
                .Where(s => !rowFigureName.IgnoreFigureRows.Contains(s["Figure"]))
                .ToList();
            subSource.ForEach(row =>
            {
                result.ImportRow(row);
            });

            return result;
        }

        public DataTable CombineAllInOneTable(DataSet dataSet)
        {
            var result = dataSet.Tables.Count > 0 ? dataSet.Tables[0].Clone() : new DataTable();
            for (int index = 0; index < dataSet.Tables.Count; index++)
            {
                var table = dataSet.Tables[index];
                foreach (DataRow row in table.Rows)
                {
                    if (!row[ColumnName.SiteGroup].Equals("CN88") || !row[ColumnName.ProductShortName].Equals("Gaming Platform"))
                    {
                        result.ImportRow(row);
                    }
                }
            }

            return result;
        }
    }
}