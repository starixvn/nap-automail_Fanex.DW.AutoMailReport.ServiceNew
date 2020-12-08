using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.DHWL.Business.Criterias;
using Fanex.DHWL.Business.Models;
using System;
using System.Data;

namespace Fanex.DHWL.Business.Services
{
    public interface DataService
    {
        TaskstatusModel QueryDWStatus();

        bool IsDataAvailable();

        DataTable QueryDailyData();

        DataTable QueryLastMonthData();

        DataTable QueryCN88();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;

        public DataServiceImp(
            IDynamicRepository repository,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth)
        {
            this.repository = repository;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
        }

        public TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public bool IsDataAvailable()
        {
            var result = false;
            if (AppConfigService.Config.ReportType == ReportType.Manual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            result = QueryDWStatus().DataIsReady;

            return result;
        }

        public DataTable QueryDailyData()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            return QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome_Credit");
        }

        private DataTable QueryRawData(DateTime fromDate, DateTime toDate, string spName)
        {
            var datatable = new DataTable();

            using (ObjectDb db = new ObjectDb(spName, 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
                db.SetParameterValue("productlist", AppConfigService.Config.ProductList);
                var dataset = db.ExecuteDataSet();
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    datatable = dataset.Tables[0];
                }
            }

            return datatable;
        }

        public DataTable QueryLastMonthData()
        {
            var fromDate = lastMonth.FromDate;
            var toDate = lastMonth.ToDate;
            return QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome_Credit")
                .AsEnumerable()
                .Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                .CopyToDataTable();
        }

        public DataTable QueryCN88()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            var datatable = new DataTable();

            using (ObjectDb db = new ObjectDb("Acc_Rpt_HouseNetIncome_CN88", 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
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