using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using Fanex.Service.Core;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface RawDataSourceService
    {
        Dictionary<string, DataTable> QueryDataSouce(SourceMapping sourceMapping);
    }

    public class RawDataSourceServiceImp : RawDataSourceService
    {
        private readonly DataSourceService dataSourceService;
        private readonly DateRangeService dateRangeService;

        public RawDataSourceServiceImp(DateRangeService dateRangeService, DataSourceService dataSourceService)
        {
            this.dateRangeService = dateRangeService;
            this.dataSourceService = dataSourceService;
        }

        public Dictionary<string, DataTable> QueryDataSouce(SourceMapping sourceMapping)
        {
            var rawDataSource = new Dictionary<string, DataTable>();
            var criteria = new BettypeProductCriteria
            {
                DateStart = dateRangeService.DateStart,
                DateEnd = dateRangeService.DateEnd,
                StatisticId = (int)sourceMapping.DataType,
                StoreProcedure = sourceMapping.StoreProcedure
            };

            var dataset = dataSourceService.QueryMultiple(criteria);
            var index = 0;
            string key = string.Empty;
            foreach (DataTable dataTable in dataset.Tables)
            {
                if (index == 0)
                {
                    key = dataTable.Rows[0]["Sport"].ToString();
                }

                if (index == 1)
                {
                    rawDataSource.Add(key, dataTable);
                    index = -1;
                }

                index++;
            }

            return rawDataSource;
        }
    }
}