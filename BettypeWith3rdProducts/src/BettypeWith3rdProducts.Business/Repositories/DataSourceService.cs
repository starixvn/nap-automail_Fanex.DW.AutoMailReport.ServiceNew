using System.Collections.Generic;
using System.Data;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data;
using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public interface DataSourceService
    {
        DataTable QueryDataSouce(SourceMapping sourceMapping, DateRange dateRange, ViewOfData viewOfData, int sportId);

        IEnumerable<SportType> QuerySports(DataType dataType);
    }

    public class DataSourceServiceImp : DataSourceService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly DateRangeServiceProvider dateRangeService;
        private readonly ViewOfDataServiceProvider viewOfDataService;

        public DataSourceServiceImp(IDynamicRepository dynamicRepository, DateRangeServiceProvider dateRangeService, ViewOfDataServiceProvider viewOfDataService)
        {
            this.dynamicRepository = dynamicRepository;
            this.dateRangeService = dateRangeService;
            this.viewOfDataService = viewOfDataService;
        }

        public DataTable QueryDataSouce(SourceMapping sourceMapping, DateRange dateRange, ViewOfData viewOfData, int sportId)
        {
            var dataTable = new DataTable();
            var date = dateRangeService.Date(dateRange);
            var view = viewOfDataService.View(viewOfData, sportId);
            var criteria = new BettypeProductCriteria
            {
                DateStart = date.DateStart,
                DateEnd = date.DateEnd,
                Sportid = view.SportId,
                Periodid = view.PeriodId,
                StatisticId = (int)sourceMapping.DataType,
                StoreProcedure = sourceMapping.StoreProcedure
            };

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var dataset = db.ExecuteDataSet();
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    dataTable = dataset.Tables[0];
                }
            }

            return dataTable;
        }

        public IEnumerable<SportType> QuerySports(DataType dataType)
        {
            return dynamicRepository.Fetch<SportType>(new SportCriteria { Include3rd = dataType == DataType.RealWiLoss });
        }
    }
}