using System.Collections.Generic;
using System.Data;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DataService
    {
        Dictionary<string, Dictionary<string, List<DataTable>>> Execute();
    }

    public class DataServiceImp : DataService
    {
        private readonly ApplicationConfigurationService appConfig;
        private readonly DataSplitterService dataSplitterService;

        public DataServiceImp(ApplicationConfigurationService appConfig, DataSplitterService dataSplitterService)
        {
            this.appConfig = appConfig;
            this.dataSplitterService = dataSplitterService;
        }

        public Dictionary<string, Dictionary<string, List<DataTable>>> Execute()
        {
            var dataSource = new Dictionary<string, Dictionary<string, List<DataTable>>>();
            var sourceMappings = appConfig.SourceMappings;
            foreach (var sourceMapping in sourceMappings)
            {
                dataSource.Add(sourceMapping.SheetName, dataSplitterService.Execute(sourceMapping));
            }

            return dataSource;
        }
    }
}