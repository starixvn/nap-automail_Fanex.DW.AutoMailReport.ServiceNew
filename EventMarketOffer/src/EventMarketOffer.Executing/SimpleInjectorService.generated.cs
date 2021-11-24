using EventMarketOffer.Business.Services;
using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;
using System.IO;

namespace EventMarketOffer.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<AppConfigurationService, AppConfigurationServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<Log, LogImp>();
            container.Register<NotificationService, NotificationServiceImp>();
        }
    }
}