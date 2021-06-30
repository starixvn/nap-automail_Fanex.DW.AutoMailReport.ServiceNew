using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface MergeProductDataService
    {
        void MergeSabaCIntoVGaming(DataTable dataTable);
    }

    public class MergeProductDataServiceImp : MergeProductDataService
    {
        private readonly List<string> productShortNames = new List<string>
        {
            "Vgaming",
            "Vgaming-JP"
        };

        public void MergeSabaCIntoVGaming(DataTable dataTable)
        {
            dataTable.AsEnumerable()
                .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.RNGCasino) && productShortNames.Contains(s[ColumnName.ProductShortName]))
                .ToList()
                .ForEach(row =>
                {
                    if (!Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals("Vgaming"))
                    {
                        row[ColumnName.ProductShortName] = "SabaC";
                        row[ColumnName.ProductName] = "Saba.club";
                        row[ColumnName.OrderByProduct] = 201;
                    }

                    if (Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals("Vgaming"))
                    {
                        row[ColumnName.ProductShortName] = "SabaC";
                        row[ColumnName.ProductName] = "Saba.club";
                        row[ColumnName.OrderByProduct] = 201;
                    }

                    if (Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals("Vgaming-JP"))
                    {
                        row[ColumnName.ProductShortName] = "SabaC-JP";
                        row[ColumnName.ProductName] = "Saba.club Jackpot Pool";
                        row[ColumnName.OrderByProduct] = 202;
                    }
                });
        }
    }
}