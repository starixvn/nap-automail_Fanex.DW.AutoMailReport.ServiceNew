using Fanex.Common;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.SoccerNonLive.Business.Criterias;
using Fanex.SoccerNonLive.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface DataService
    {
        DataTable QueryData();

        bool IsDataAvailable();

        TaskstatusModel QueryDWStatus();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;
        private decimal to = 0;
        private decimal wl = 0;
        private decimal value = 0;

        public DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public DataTable QueryData()
        {
            var datatable = new DataTable();
            var criteria = new Criteria
            {
                FromDate = dateService.DateTime.FromDate,
                ToDate = dateService.DateTime.ToDate
            };

            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey(), 0))
            {
                db.SetParameterValue("fromdate", criteria.FromDate);
                db.SetParameterValue("todate", criteria.ToDate);

                var dataset = db.ExecuteDataSet();
                if (dataset != null && dataset.Tables.Count > 0)
                {
                    datatable = dataset.Tables[0];
                }
            }

            var pivotTable = PivotTable(datatable);
            var result = CalcTotalByLeague(pivotTable);

            return result;
        }

        private DataTable CalcTotalByLeague(DataTable dataSource)
        {
            var result = dataSource.Clone();
            dataSource.AsEnumerable().GroupBy(g => new
            {
                LeagueName = g["LeagueName"].ToString()
            }).ToList().ForEach(league =>
            {
                league.ToList().ForEach(row =>
                {
                    result.ImportRow(row);
                });

                var leagueTotalRow = result.NewRow();
                AssignLeagueTotalRow(leagueTotalRow, league, dataSource);
                result.Rows.Add(leagueTotalRow);
            });

            var grandTotalRow = result.NewRow();
            AssignGrandTotalRow(grandTotalRow, dataSource);
            result.Rows.Add(grandTotalRow);

            return result;
        }

        private void AssignLeagueTotalRow(DataRow leagueTotalRow, IGrouping<dynamic, DataRow> league, DataTable dataSource)
        {
            leagueTotalRow["LeagueName"] = $"{league.Key.LeagueName} Total";
            for (int columnIndex = 2; columnIndex < dataSource.Columns.Count; columnIndex++)
            {
                value = league.Sum(s => Converter.To<decimal>(s[columnIndex]));
                if (dataSource.Columns[columnIndex].ColumnName.Contains("Turnover"))
                {
                    to = value;
                }
                else if (dataSource.Columns[columnIndex].ColumnName.Contains("Income"))
                {
                    wl = value;
                }
                else if (dataSource.Columns[columnIndex].ColumnName.Contains("Margin"))
                {
                    value = to == 0 ? 0 : wl / to;
                }

                leagueTotalRow[columnIndex] = value;
            }
        }

        private void AssignGrandTotalRow(DataRow grandTotalRow, DataTable dataSource)
        {
            grandTotalRow["LeagueName"] = "Grand Total";
            for (int columnIndex = 2; columnIndex < dataSource.Columns.Count; columnIndex++)
            {
                value = dataSource.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnIndex]));
                if (dataSource.Columns[columnIndex].ColumnName.Contains("Turnover"))
                {
                    to = value;
                }
                else if (dataSource.Columns[columnIndex].ColumnName.Contains("Income"))
                {
                    wl = value;
                }
                else if (dataSource.Columns[columnIndex].ColumnName.Contains("Margin"))
                {
                    value = to == 0 ? 0 : wl / to;
                }

                grandTotalRow[columnIndex] = value;
            }
        }

        private DataTable PivotTable(DataTable datatable)
        {
            datatable.Columns.Remove("MatchID");
            var pivotDataTable = new DataTable();
            var newColumns = NewColumns(datatable, pivotDataTable);

            datatable.AsEnumerable().GroupBy(g => new
            {
                LeagueName = g["LeagueName"].ToString(),
                MatchEvent = g["MatchEvent"].ToString()
            })

            .ToList().ForEach(league =>
            {
                var newRow = pivotDataTable.NewRow();
                foreach (var column in newColumns)
                {
                    var colmnName = column.Key;
                    var leagueName = league.Key.LeagueName;
                    var matchEvent = league.Key.MatchEvent;
                    AssignDataRow(newRow, colmnName, league, leagueName, matchEvent);
                }

                pivotDataTable.Rows.Add(newRow);
            });

            return pivotDataTable;
        }

        private void AssignDataRow(
            DataRow newRow,
            string colmnName,
            IGrouping<object, DataRow> league,
            string leagueName,
            string matchEvent)
        {
            if (colmnName.Contains("^") && !colmnName.Contains("Margin"))
            {
                var col1 = colmnName.Split('^')[0];
                var col2 = colmnName.Split('^')[1];
                value = league.Where(s => s["BetTypeName"].Equals(col1)).Sum(s => Converter.To<decimal>(s[col2]));
                if (col2.Contains("Turnover"))
                {
                    to = value;
                }
                else if (col2.Contains("Income"))
                {
                    wl = value;
                }

                newRow[colmnName] = value;
            }
            else if (colmnName.Contains("^") && colmnName.Contains("BetCount"))
            {
                var col1 = colmnName.Split('^')[0];
                var col2 = colmnName.Split('^')[1];
                value = league.Where(s => s["BetTypeName"].Equals(col1)).Sum(s => Converter.To<decimal>(s[col2]));
                newRow[colmnName] = value;
            }
            else if (colmnName.Contains("Margin") && !colmnName.Contains("GrandTotal"))
            {
                var margin = to == 0 ? 0 : wl / to;
                newRow[colmnName] = margin;
            }
            else if (colmnName.Contains("Virtual-GrandTotal"))
            {
                wl = league.Where(s => s["LeagueName"].Equals(leagueName)).Sum(s => Converter.To<decimal>(s["VirtualIncome"]));
                to = league.Where(s => s["LeagueName"].Equals(leagueName)).Sum(s => Converter.To<decimal>(s["VirtualTurnover"]));
                var margin = to == 0 ? 0 : wl / to;
                newRow["Virtual-GrandTotal-Income"] = wl;
                newRow["Virtual-GrandTotal-Turnover"] = to;
                newRow[colmnName] = margin;
            }
            else if (colmnName.Contains("Real-GrandTotal"))
            {
                wl = league.Where(s => s["LeagueName"].Equals(leagueName)).Sum(s => Converter.To<decimal>(s["RealIncome"]));
                to = league.Where(s => s["LeagueName"].Equals(leagueName)).Sum(s => Converter.To<decimal>(s["RealTurnover"]));
                var margin = to == 0 ? 0 : wl / to;
                newRow["Real-GrandTotal-Income"] = wl;
                newRow["Real-GrandTotal-Turnover"] = to;
                newRow[colmnName] = margin;
            }
            else if (colmnName.Contains("GrandTotal-BetCount"))
            {
                newRow[colmnName] = league.Sum(s => Converter.To<int>(s["BetCount"]));
            }
            else
            {
                newRow[colmnName] = colmnName.Contains("LeagueName") ? leagueName : matchEvent;
            }
        }

        private static Dictionary<string, Type> NewColumns(DataTable datatable, DataTable pivotDataTable)
        {
            var newColumns = new Dictionary<string, Type>
            {
                { "LeagueName", typeof(string) },
                { "MatchEvent", typeof(string) }
            };

            var combineColumns = new List<string> { "VirtualTurnover", "VirtualIncome", "VirtualMargin", "RealTurnover", "RealIncome", "RealMargin", "BetCount" };
            var columnsPivot = new List<string> { "Handicap", "Over/Under", "1H Handicap", "1H Over/Under", "FT.1X2", "1H 1X2" };

            for (var i = 0; i < columnsPivot.Count; i++)
            {
                for (var j = 0; j < combineColumns.Count; j++)
                {
                    var newColumnName = $"{columnsPivot[i]}^{combineColumns[j]}";
                    newColumns.Add(newColumnName, typeof(decimal));
                }
            }

            newColumns.Add("Virtual-GrandTotal-Turnover", typeof(decimal));
            newColumns.Add("Virtual-GrandTotal-Income", typeof(decimal));
            newColumns.Add("Virtual-GrandTotal-Margin", typeof(decimal));
            newColumns.Add("Real-GrandTotal-Turnover", typeof(decimal));
            newColumns.Add("Real-GrandTotal-Income", typeof(decimal));
            newColumns.Add("Real-GrandTotal-Margin", typeof(decimal));
            newColumns.Add("GrandTotal-BetCount", typeof(int));
            foreach (var column in newColumns)
            {
                pivotDataTable.Columns.Add(column.Key, column.Value);
            }

            return newColumns;
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

        public TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }
    }
}