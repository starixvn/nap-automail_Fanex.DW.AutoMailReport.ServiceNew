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

        DataTable QueryDHWLData();

        DataTable QueryLastMonthData();

        DataTable QueryReconciliation();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;
        private readonly BaseDataTable baseDataTable;

        public DataServiceImp(
            IDynamicRepository repository,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth,
            BaseDataTable baseDataTable)
        {
            this.repository = repository;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
            this.baseDataTable = baseDataTable;
        }

        public TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        private bool CheckLicenseeDataIsReady()
        {
            return repository.Get<LicenseeCheckDataReady>(new LiccenseeCheckDataReadyCriteria()).IsDataYesterdayReady;
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

        public DataTable QueryDHWLData()
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

        public DataTable QueryLastMonthData()
        {
            var fromDate = lastMonth.FromDate;
            var toDate = lastMonth.ToDate;
            return QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome")
                .AsEnumerable()
                .Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                .CopyToDataTable();
        }

        public DataTable QueryReconciliation()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;

            var dataTable = new DataTable();

            using (ObjectDb db = new ObjectDb("Acc_Rpt_HouseNetIncome_Reconciliation", 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);

                var dataSet = db.ExecuteDataSet();
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];
                }
            }

            return dataTable;
        }
    }
}