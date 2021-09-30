using OfficeOpenXml;
using System;
using System.Data;

namespace Fanex.DHWL.Business.Services.MonthlyReconciliation
{
    public interface MonthlyBAExcelService
    {
        void RenderBA(ExcelWorksheet template, DataTable dataBA, DateTime toDate);
    }

    public class MonthlyBAExcelServiceImp : MonthlyBAExcelService
    {
        public void RenderBA(ExcelWorksheet template, DataTable dataBA, DateTime toDate)
        {
            template.Cells["A2"].Value = $"Monthly Licensee BA Win Loss Report For {toDate:MMM yyyy}";
            template.Cells["A3"].Value = $"Generated on {DateTime.Now:dd MMM yyyy hh:mm:ss tt}";
            template.Cells["A6"].LoadFromDataTable(dataBA, false);

            FormatBody(template, dataBA);
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
                template.Row(rowIndex).Height = 15;
                rowTableIndex++;
            }
        }
    }
}