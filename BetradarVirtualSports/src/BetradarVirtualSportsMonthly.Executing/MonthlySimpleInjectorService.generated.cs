using System.IO;
using BetradarVirtualSportsMonthly.Business.Services;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using OfficeOpenXml;
using SimpleInjector;

namespace BetradarVirtualSportsMonthly.Executing
{
    internal static class MonthlySimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailService, MailMonthlyService>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<LogService, LogServiceImp>();            
            container.Register<DateService, DateServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<DataService, RepositoryServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Monthly-Template.xlsx")));
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
        }
    }
}