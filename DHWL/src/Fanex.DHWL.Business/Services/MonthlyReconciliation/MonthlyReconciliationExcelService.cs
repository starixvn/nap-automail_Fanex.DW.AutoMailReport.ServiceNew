using OfficeOpenXml;
using System;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.MonthlyReconciliation
{
    public interface MonthlyReconciliationExcelService
    {
        void RenderReconciliation(ExcelWorksheet template, DataTable dataConciliation, DateTime toDate);
    }

    public class MonthlyReconciliationExcelServiceImp : MonthlyReconciliationExcelService
    {
        public void RenderReconciliation(ExcelWorksheet template, DataTable dataConciliation, DateTime toDate)
        {
            template.Cells["A2"].Value = $"Monthly Licensee Rebate & Discount Report For {toDate:MMM yyyy}";
            template.Cells["A3"].Value = $"Generated on {DateTime.Now:dd MMM yyyy hh:mm:ss tt}";
            template.Cells["A6"].LoadFromDataTable(dataConciliation, false);

            FormatBody(template, dataConciliation);
        }

        private static void FormatBody(ExcelWorksheet template, DataTable dataConciliation)
        {
            var startRowIndex = 7;
            var maxRowIndex = dataConciliation.Rows.Count + startRowIndex - 2;
            var startColumnIndex = 1;
            var columnCount = dataConciliation.Columns.Count;

            for (var columnIndex = startColumnIndex; columnIndex <= columnCount; columnIndex++)
            {
                template.Cells[startRowIndex, columnIndex, maxRowIndex, columnIndex].StyleID = template.Cells[startRowIndex, columnIndex].StyleID;
            }

            var rowTableIndex = 0;
            for (var rowIndex = startRowIndex - 1; rowIndex <= maxRowIndex; rowIndex++)
            {
                var housePT = dataConciliation.Rows[rowTableIndex]["HousePT"].ToString().Split('|');
                var chargeRate = dataConciliation.Rows[rowTableIndex]["ChargeRate"].ToString().Split('|');
                var max = Math.Max(housePT.Count(), chargeRate.Count());

                template.Cells[rowIndex, 7].Value = string.Join(Environment.NewLine, housePT);
                template.Cells[rowIndex, 10].Value = string.Join(Environment.NewLine, chargeRate);

                template.Row(rowIndex).Height = 15 * max;
                rowTableIndex++;
            }
        }
    }
}