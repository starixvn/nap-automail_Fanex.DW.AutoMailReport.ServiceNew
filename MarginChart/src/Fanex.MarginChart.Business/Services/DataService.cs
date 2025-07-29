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

        DataTable ThousandScale(DataTable source, int startScaleCol);
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

            var result = QueryDWStatus().DataIsReady;

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
                db.SetParameterValue("islicperf", false);
                db.SetParameterValue("basecurrencyname", "MYR");
                db.SetParameterValue("deposittype", 2);
                db.SetParameterValue("datamode", AppConfigService.Config.DataMode);
                db.SetParameterValue("excluproduct", "VS6");

                var dataSet = db.ExecuteDataSet();
                dataTable = baseDataTable.CombineAllInOneTable(dataSet);
            }

            return dataTable;
        }

        private static DataTable Clone(DataTable source)
        {
            var result = source.Clone();
            foreach (DataRow row in source.Rows)
            {
                result.ImportRow(row);
            }

            return result;
        }

        public DataTable ThousandScale(DataTable source, int startScaleCol)
        {
            var sourceClone = Clone(source);
            var result = sourceClone.Clone();
            foreach (DataRow row in sourceClone.Rows)
            {
                var colIndex = 0;
                foreach (DataColumn column in sourceClone.Columns)
                {
                    if (colIndex >= startScaleCol)
                    {
                        if (decimal.TryParse(row[column].ToString(), out var value))
                        {
                            row[column] = value / 1000;
                        }
                    }

                    colIndex++;
                }

                result.ImportRow(row);
            }

            return result;
        }
    }
}