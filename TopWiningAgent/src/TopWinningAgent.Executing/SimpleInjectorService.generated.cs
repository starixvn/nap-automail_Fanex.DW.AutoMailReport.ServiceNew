using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;
using System.IO;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\TopWinningTemplate.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<Log, LogImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<NotificationService, NotificationServiceImp>();
        }
    }
}