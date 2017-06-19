using System.IO;
using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using OfficeOpenXml;
using SimpleInjector;

namespace BettypeWith3rdProducts.DatabaseTests
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeServiceProvider, DateRangeServiceProviderImp>();
            container.Register<ParameterService, ParameterServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<SportSplitterService, SportSplitterServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ExcelHeaderService, ExcelHeaderServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailContentBuilder, MailContentBuilderImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<LogService, LogServiceImp>();

            container.Register<DataService, DataServiceImp>();
        }
    }
}