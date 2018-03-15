using System.IO;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using OfficeOpenXml;
using SimpleInjector;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\TopWinningTemplate.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
        }
    }
}