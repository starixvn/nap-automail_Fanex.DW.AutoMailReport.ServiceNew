using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.DHWL.Business.Criterias;
using Fanex.DHWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DataService
    {
        TaskstatusModel QueryDwStatus();

        List<string> CheckDataReadyDaily();

        List<string> CheckDataReadyMonthly();

        DataTable QueryDhwlData();

        DataTable QueryDhwlDataPreviousYear();

        DataTable QueryLastMonthData();

        DataSet QueryReconciliation();

        DataTable QueryDhwlMonthlyChart();

        List<GetProductInfoModel> QueryGetProductInfo(int productId);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private readonly DateTimeService lastMonth;
        private readonly BaseDataTable baseDataTable;
        private readonly ExcelRawService excelRawService;

        public DataServiceImp(
            IDynamicRepository repository,
            DateTimeServiceProvider dateService,
            DateTimeService lastMonth,
            BaseDataTable baseDataTable,
            ExcelRawService excelRawService)
        {
            this.repository = repository;
            this.dateService = dateService;
            this.lastMonth = lastMonth;
            this.baseDataTable = baseDataTable;
            this.excelRawService = excelRawService;
        }

        public TaskstatusModel QueryDwStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public List<string> CheckDataReadyDaily()
        {
            var result = new List<string>();
            if (AppConfigService.Config.ReportType == ReportType.Manual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return result;
            }

            var dataReady = QueryDwStatus().DataIsReady;
            if (!dataReady)
            {
                result.Add("- DataIsNotReady, TaskId = 77");
            }

            var hniResultReady = HniResultReady();
            if (!hniResultReady)
            {
                var value = AppConfigService.Config.PerformanceThreshold / 1000;
                result.Add(
                    $"- Detected a significant discrepancy (> {value}) between MTD up to the day before yesterday, generated in yesterday, and MTD up to the day before yesterday, generated in today.");
            }

            return result;
        }

        public List<string> CheckDataReadyMonthly()
        {
            var result = new List<string>();
            if (AppConfigService.Config.ReportType == ReportType.Manual || !AppConfigService.Config.IsCheckEnoughData)
            {
                return result;
            }

            var dataReady = QueryDwStatus().DataIsReady;
            if (!dataReady)
            {
                result.Add("DataIsNotReady, TaskId = 77");
            }

            return result;
        }

        private bool OandaReady()
        {
            var result = false;
            using (ObjectDb db = new ObjectDb("Task_CheckOANDASettingsReadiness", 0))
            {
                var dataset = db.ExecuteDataSet();
                result = Convert.ToInt32(dataset.Tables[0].Rows[0][0]) == 1;
            }

            return result;
        }

        private static bool HniResultReady()
        {
            var result = false;
            using (ObjectDb db = new ObjectDb("Task_CheckHNIResultDatasetReadiness", 0))
            {
                db.SetParameterValue("performancethreshold", AppConfigService.Config.PerformanceThreshold);
                var dataset = db.ExecuteDataSet();
                result = Convert.ToInt32(dataset.Tables[0].Rows[0][0]) == 1;
            }

            return result;
        }

        public DataTable QueryDhwlData()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            return QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome", false);
        }

        public DataTable QueryDhwlDataPreviousYear()
        {
            var previousYear = new DateTimePreviousYearServiceImp(dateService);
            var fromDate = previousYear.FromDate;
            var toDate = previousYear.ToDate;
            DataTable dataTable;

            using (var db = new ObjectDb("Acc_Rpt_HouseNetIncome", 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
                db.SetParameterValue("islicperf", false);

                db.SetParameterValue("basecurrencyname", "MYR");
                db.SetParameterValue("deposittype", 2);
                db.SetParameterValue("datamode", 3);

                var dataSet = db.ExecuteDataSet();
                dataTable = baseDataTable.CombineAllInOneTable(dataSet);
            }

            excelRawService.Export(dataTable, "LastYear", fromDate, toDate);

            return dataTable;
        }

        private DataTable QueryRawData(DateTime fromDate, DateTime toDate, string spName, bool isLicPerf)
        {
            DataTable dataTable;

            using (ObjectDb db = new ObjectDb(spName, 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
                db.SetParameterValue("islicperf", isLicPerf);

                db.SetParameterValue("basecurrencyname", "MYR");
                db.SetParameterValue("deposittype", 2);
                db.SetParameterValue("datamode", AppConfigService.Config.DataMode);

                var dataSet = db.ExecuteDataSet();
                dataTable = baseDataTable.CombineAllInOneTable(dataSet);
            }

            return dataTable;
        }

        public DataTable QueryLastMonthData()
        {
            var fromDate = lastMonth.FromDate;
            var toDate = lastMonth.ToDate;
            var data = QueryRawData(fromDate, toDate, "Acc_Rpt_HouseNetIncome", false);
            if (data.Rows.Count == 0)
            {
                return data;
            }

            var result = data.AsEnumerable()
                .Where(s => !s[ColumnName.Figure].ToString().Equals(RowFigureName.HouseTurnOver))
                .CopyToDataTable();

            return result;
        }

        public DataSet QueryReconciliation()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            var result = new DataSet();
            using (ObjectDb db = new ObjectDb("Acc_Rpt_HouseNetIncome_Reconciliation", 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);

                result = db.ExecuteDataSet();
            }

            return result;
        }

        public List<GetProductInfoModel> QueryGetProductInfo(int productId)
        {
            var criteria = new GetProductInfoCriteria { ProductID = productId };
            return repository.Fetch<GetProductInfoModel>(criteria).ToList();
        }

        public DataTable QueryDhwlMonthlyChart()
        {
            var fromDate = dateService.DateTime.FromDate;
            var toDate = dateService.DateTime.ToDate;
            DataTable dataTable;

            using (var db = new ObjectDb("Acc_Rpt_HouseNetIncome", 0))
            {
                db.SetParameterValue("fromdate", fromDate);
                db.SetParameterValue("todate", toDate);
                db.SetParameterValue("islicperf", false);

                db.SetParameterValue("basecurrencyname", "MYR");
                db.SetParameterValue("deposittype", 2);
                db.SetParameterValue("datamode", 4);

                var dataSet = db.ExecuteDataSet();
                dataTable = baseDataTable.CombineAllInOneTable(dataSet);
            }

            excelRawService.Export(dataTable, "DHWL-YTD-Chart", fromDate, toDate);

            return dataTable;
        }
    }
}