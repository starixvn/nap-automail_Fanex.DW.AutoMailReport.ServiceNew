using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services.LicenseePerformance
{
    public interface MergeProductDataService
    {
        void MergeVGamingIntoSabaC(DataTable dataTable);

        void MoveSabaProIntoSportsbook(DataTable dataTable);
    }

    public class MergeProductDataServiceImp : MergeProductDataService
    {
        private readonly List<string> productShortNames = new List<string>
        {
            "Vgaming",
            "Vgaming-JP"
        };

        public void MergeVGamingIntoSabaC(DataTable dataTable)
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

        public void MoveSabaProIntoSportsbook(DataTable dataTable)
        {
            dataTable.AsEnumerable()
                .Where(s => s[ColumnName.ProductShortName].Equals("SabaPro"))
                .ToList()
                .ForEach(row =>
                {
                    row[ColumnName.ProductGroup] = "Sportsbook";
                    row[ColumnName.ProductCategory] = "Soccer";
                    row[ColumnName.ProductName] = "Sportsbook";
                });
        }
    }
}