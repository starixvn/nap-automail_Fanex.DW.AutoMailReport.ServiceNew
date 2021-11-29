using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Data;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface RawDataSourceService
    {
        Dictionary<string, DataTable> QueryDataSouce(SourceMapping sourceMapping);
    }

    public class RawDataSourceServiceImp : RawDataSourceService
    {
        private readonly DateRangeService dateRangeService;
        private readonly IDynamicRepository dynamicRepository;

        public RawDataSourceServiceImp(DateRangeService dateRangeService, IDynamicRepository dynamicRepository)
        {
            this.dateRangeService = dateRangeService;
            this.dynamicRepository = dynamicRepository;
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

            var dataset = QueryData(criteria);
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

        private DataSet QueryData(BettypeProductCriteria criteria)
        {
            var ds = new DataSet();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                db.SetParameterValue("datestart", criteria.DateStart);
                db.SetParameterValue("dateend", criteria.DateEnd);
                db.SetParameterValue("statisticid", criteria.StatisticId);

                ds = db.ExecuteDataSet();
            }

            return ds;
        }
    }
}