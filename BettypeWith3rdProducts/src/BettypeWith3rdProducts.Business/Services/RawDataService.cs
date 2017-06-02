using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;
using BettypeWith3rdProducts.Business.Repositories;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface RawDataService
    {
        Dictionary<string, DataTable> Query(SourceMapping sourceMapping);
    }

    public class RawDataServiceImp : RawDataService
    {
        private readonly DataSourceService dataSourceService;
        private readonly ApplicationConfigurationService appConfig;

        private Dictionary<string, DataTable> dataSource;

        public RawDataServiceImp(DataSourceService dataSourceService, ApplicationConfigurationService appConfig)
        {
            this.dataSourceService = dataSourceService;
            this.appConfig = appConfig;            
        }

        public Dictionary<string, DataTable> Query(SourceMapping sourceMapping)
        {
            dataSource = new Dictionary<string, DataTable>();
            QueryDataBySpecialCase(sourceMapping);
            QueryDataBySport(sourceMapping);
            return dataSource;
        }

        private void QueryDataBySpecialCase(SourceMapping sourceMapping)
        {
            Enum.GetValues(typeof(ViewOfData))
              .Cast<ViewOfData>()
              .Select(v => v.ToString())
              .ToList().ForEach(view =>
              {
                  ViewOfData viewOfData;
                  Enum.TryParse(view, true, out viewOfData);

                  if (viewOfData != ViewOfData.Detail)
                  {
                      var dataTable = dataSourceService.QueryDataSouce(sourceMapping, appConfig.ReportType, viewOfData, 0);
                      dataSource.Add(view, dataTable);
                  }
              });
        }

        private void QueryDataBySport(SourceMapping sourceMapping)
        {
            var sports = dataSourceService.QuerySports(sourceMapping.DataType);
            foreach (var sport in sports)
            {
                if (dataSource.ContainsKey(sport.Sport))
                {
                    continue;
                }

                var dataTable = dataSourceService.QueryDataSouce(sourceMapping, appConfig.ReportType, ViewOfData.Detail, sport.SportId);
                if (dataTable.Columns.Count > 0 && dataTable.Rows.Count > 0)
                {
                    dataSource.Add(sport.Sport, dataTable);
                }
            }
        }
    }
}