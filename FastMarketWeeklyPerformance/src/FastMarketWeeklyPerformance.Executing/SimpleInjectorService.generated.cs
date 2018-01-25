using System.IO;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Services;
using OfficeOpenXml;
using SimpleInjector;

namespace FastMarketWeeklyPerformance.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<LogService, LogServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
        }
    }
}