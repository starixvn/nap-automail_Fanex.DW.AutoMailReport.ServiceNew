using System.IO;
using EventMarketOffer.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using OfficeOpenXml;
using SimpleInjector;

namespace EventMarketOffer.IntegrationTests
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<AppConfigurationService, AppConfigurationServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<LogService, LogServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();

            container.Register<RepositoryService, DataServiceFake>();
            container.Register<DataService, DataServiceFake>();
        }
    }
}