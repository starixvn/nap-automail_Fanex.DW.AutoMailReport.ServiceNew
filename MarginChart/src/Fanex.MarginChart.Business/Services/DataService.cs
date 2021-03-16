using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.MarginChart.Business.Criterias;
using Fanex.MarginChart.Business.Models;
using System;
using System.Data;

namespace Fanex.MarginChart.Business.Services
{
    public interface DataService
    {
        TaskstatusModel QueryDWStatus();

        bool IsDataAvailable();

        DataTable QueryMarginChartData();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private readonly BaseDataTable baseDataTable;

        public DataServiceImp(
            IDynamicRepository repository,
            DateTimeServiceProvider dateService,
            BaseDataTable baseDataTable)
        {
            this.repository = repository;
            this.dateService = dateService;
            this.baseDataTable = baseDataTable;
        }

        public TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public bool IsDataAvailable()
        {
            if (AppConfigService.Config.ReportType == ReportType.Manual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return true;
            }

            var checkLicenseeDataIsReady = CheckLicenseeDataIsReady();
            var result = QueryDWStatus().DataIsReady && checkLicenseeDataIsReady;

            return result;
        }

        private bool CheckLicenseeDataIsReady()
        {
            return repository.Get<LicenseeCheckDataReady>(new LiccenseeCheckDataReadyCriteria()).IsDataYesterdayReady;
        }

        public DataTable QueryMarginChartData()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            return QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome");
        }

        private DataTable QueryRawData(DateTime fromDate, DateTime toDate, string spName)
        {
            var dataTable = new DataTable();

            using (ObjectDb db = new ObjectDb(spName, 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
                db.SetParameterValue("productlist", AppConfigService.Config.ProductList);
                var dataSet = db.ExecuteDataSet();
                dataTable = baseDataTable.CombineAllInOneTable(dataSet);
            }

            return dataTable;
        }
    }
}