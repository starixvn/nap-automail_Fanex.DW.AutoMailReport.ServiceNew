using System.IO;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using Fanex.UpdateExchangeRate.Business.Services;
using OfficeOpenXml;
using SimpleInjector;

namespace Fanex.UpdateExchangeRate.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<Business.Services.MailService, MailServiceImp>();            
            container.Register<LogService, LogServiceImp>();
            container.Register<NotificationService, NotificationServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<DataService, DataServiceImp>();            
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
        }
    }
}