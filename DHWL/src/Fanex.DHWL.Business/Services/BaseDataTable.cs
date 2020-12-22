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
                    result.ImportRow(row);
                }
            }

            return result;
        }
    }
}