using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using SimpleInjector;

namespace BettypeWith3rdProducts.DatabaseTests
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<DateRangeServiceProvider, DateRangeServiceProviderImp>();
            container.Register<ViewOfDataServiceProvider, ViewOfDataServiceProviderImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<RawDataService, RawDataServiceImp>();
            container.Register<DataSplitterService, DataSplitterServiceImp>();
            container.Register<DataService, DataServiceImp>();
        }
    }
}