using Fanex.Data.Repository;
using FastMarketWeeklyPerformance.Business.Services;
using OfficeOpenXml;
using SimpleInjector;
using System.IO;

namespace FastMarketWeeklyPerformance.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();

            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<Log, LogImp>();
            container.Register<NotificationService, NotificationServiceImp>();
        }
    }
}