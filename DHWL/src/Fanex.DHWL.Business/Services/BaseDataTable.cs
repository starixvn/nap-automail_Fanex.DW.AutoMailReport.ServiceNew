using Fanex.DHWL.Business.Models;
using System.Data;

namespace Fanex.DHWL.Business.Services
{
    public interface BaseDataTable
    {
        DataTable Clone(DataTable source);

        DataTable CloneWithoutIgnoreFigureRows(DataTable source);
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
    }
}