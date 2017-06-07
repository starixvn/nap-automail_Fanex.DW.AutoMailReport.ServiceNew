using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace BettypeWith3rdProducts.UnitTests.Services
{
    [Category("BettypeWith3rdProducts.UnitTests.Services RawDataServiceUnitTests")]
    public class RawDataServiceUnitTests
    {
        private DataTable CreateDataSource()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Date", typeof(DateTime));
            var dataRow = dataTable.NewRow();
            dataRow["Date"] = new DateTime(2017, 01, 01);
            dataTable.Rows.Add(dataRow);

            return dataTable;
        }

        private Dictionary<string, DataTable> CreateExpected()
        {
            var dataSource = CreateDataSource();
            return new Dictionary<string, DataTable>
            {
                { "All", dataSource },
                { "Summary", dataSource },
                { "Soccer", dataSource },
            };
        }
    }
}