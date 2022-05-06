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
        private readonly DataService dataService;

        public MergeProductDataServiceImp(DataService dataService)
        {
            this.dataService = dataService;
        }

        public void MergeVGamingIntoSabaC(DataTable dataTable)
        {
            var productShortNames = new List<string>();
            var pSourceId = 66;
            var pSource = dataService.QueryGetProductInfo(pSourceId);
            var pSourceOrder = 220;
            pSource.ForEach(p =>
            {
                productShortNames.Add(p.ProductShortName);
            });

            var productId = 42;
            var products = dataService.QueryGetProductInfo(productId);
            var orderByProduct = 201;

            dataTable.AsEnumerable()
                .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.RNGCasino) && productShortNames.Contains(s[ColumnName.ProductShortName].ToString().Trim()))
                .ToList()
                .ForEach(row =>
                {
                    pSourceOrder = 220;
                    var pSourceInfo = pSource.FirstOrDefault(s => s.OrderByProduct == pSourceOrder);
                    if (!Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals(pSourceInfo.ProductShortName))
                    {
                        orderByProduct = 201;
                        var info = products.FirstOrDefault(s => s.OrderByProduct == orderByProduct);
                        row[ColumnName.ProductShortName] = info.ProductShortName;
                        row[ColumnName.ProductName] = info.ProductName;
                        row[ColumnName.OrderByProduct] = orderByProduct;
                    }

                    if (Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals(pSourceInfo.ProductShortName))
                    {
                        orderByProduct = 201;
                        var info = products.FirstOrDefault(s => s.OrderByProduct == orderByProduct);
                        row[ColumnName.ProductShortName] = info.ProductShortName;
                        row[ColumnName.ProductName] = info.ProductName;
                        row[ColumnName.OrderByProduct] = orderByProduct;
                    }

                    pSourceOrder = 221;
                    pSourceInfo = pSource.FirstOrDefault(s => s.OrderByProduct == pSourceOrder);
                    if (Convert.ToBoolean(row[ColumnName.IsDeposit]) && row[ColumnName.ProductShortName].Equals(pSourceInfo.ProductShortName))
                    {
                        orderByProduct = 202;
                        var info = products.FirstOrDefault(s => s.OrderByProduct == orderByProduct);
                        row[ColumnName.ProductShortName] = info.ProductShortName;
                        row[ColumnName.ProductName] = info.ProductName;
                        row[ColumnName.OrderByProduct] = orderByProduct;
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