using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.SBABICustomerPerformance.Business.Criterias;
using Fanex.SBABICustomerPerformance.Business.Models;
using System.Data;

namespace Fanex.SBABICustomerPerformance.Business.Services
{
    public interface DataService
    {
        DataTable QueryRawData();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;

        public DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public DataTable QueryRawData()
        {
            var datatable = new DataTable();
            var criteria = new Criteria
            {
                WinlostDate = dateService.DateTime.WinlostDate
            };

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                db.SetParameterValue("winlostdate", criteria.WinlostDate);

                var dataset = db.ExecuteDataSet();
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    datatable = dataset.Tables[0];
                }
            }

            return datatable;
        }
    }
}