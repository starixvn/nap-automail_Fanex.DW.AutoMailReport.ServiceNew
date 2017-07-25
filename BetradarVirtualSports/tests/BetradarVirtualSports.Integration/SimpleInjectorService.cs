using System.IO;
using BetradarVirtualSports.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using OfficeOpenXml;
using SimpleInjector;

namespace BetradarVirtualSports.Integration
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Daily-Template.xlsx")));
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<DataDailyService, RepositoryServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailService, MailDailyService>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<LogService, LogServiceImp>();
        }
    }
}