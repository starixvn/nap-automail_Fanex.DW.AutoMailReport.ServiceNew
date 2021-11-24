using BetradarVirtualSportsDaily.Business.Services;
using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;
using System.IO;

namespace BetradarVirtualSports.Executing
{
    internal static class DailySimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Daily-Template.xlsx")));
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<DateService, DateServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<NotificationService, NotificationServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<Log, LogImp>();
        }
    }
}