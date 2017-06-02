using System.IO;
using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using BettypeWith3rdProducts.BusinessSubstitute;
using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;

namespace BettypeWith3rdProducts.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeServiceProvider, DateRangeServiceProviderImp>();
            container.Register<ViewOfDataServiceProvider, ViewOfDataServiceProviderImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<RawDataService, RawDataServiceImp>();

            // container.Register<DataService, DataServiceImp>();
            container.Register<DataService, DataServiceSubstitute>();
            container.Register<DataSplitterService, DataSplitterServiceImp>();
        }
    }
}