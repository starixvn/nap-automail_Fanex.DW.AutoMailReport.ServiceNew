using Fanex.DHWL.Business.Models;
using System.Data;

namespace Fanex.DHWL.Business.Services
{
    public interface BaseDataTable
    {
        DataTable Clone(DataTable source);

        DataTable CloneWithoutIgnoreFigureRows(DataTable source);

        DataTable CloneRoyaltyFees(DataTable source);

        DataTable CombineAllInOneTable(DataSet dataSet);

        DataTable ThousandScale(DataTable source);
    }

    public class BaseDataTableImp : BaseDataTable
    {
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
            foreach (DataRow row in source.Rows)
            {
                if (RowFigureName.RoyaltyFees.Contains(row["Figure"].ToString()))
                {
                    result.ImportRow(row);
                }
            }

            return result;
        }

        public DataTable CloneWithoutIgnoreFigureRows(DataTable source)
        {
            var result = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                if (!RowFigureName.IgnoreFigureRows.Contains(row["Figure"].ToString()))
                {
                    result.ImportRow(row);
                }
            }

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
                    if (!row[ColumnName.SiteGroup].Equals("CN88"))
                    {
                        result.ImportRow(row);
                    }
                }
            }

            return result;
        }

        public DataTable ThousandScale(DataTable source)
        {
            var result = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                var colIndex = 0;
                foreach (DataColumn column in source.Columns)
                {
                    if (colIndex > 9)
                    {
                        if (decimal.TryParse(row[column].ToString(), out var value))
                        {
                            row[column] = value / 1000;
                        }
                    }

                    colIndex++;
                }

                result.ImportRow(row);
            }

            return result;
        }
    }
}