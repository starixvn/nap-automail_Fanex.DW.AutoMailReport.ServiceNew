using System.IO;
using Fanex.Data.Repository;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Services;
using Fanex.Service.Core;
using MaxLossMonitoring.Business.Services;
using OfficeOpenXml;
using SimpleInjector;

namespace MaxLossMonitoring.UnitTests
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\LossMonitoringTemplate.xlsx")));
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<DataService, DataServiceImp>();
            container.Register<MaxLossRepositoryService, MaxLossRepositoryServiceImp>();
            container.Register<RepositoryService, MaxLossRepositoryServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<MailServiceLossMonitoringService, MailServiceLossMonitoringServiceImp>();
            container.Register<DistributedNotificationLossMonitoringService, DistributedNotificationLossMonitoringServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<MessageContentService, MessageContentServiceImp>();
            container.Register<MessageService, MessageServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();

            container.Register<LetstalkClientService, LetstalkClientServiceImp>();
            container.Register<ApplicationSettingService, ApplicationSettingServiceImp>();
            container.Register<WebApiClientService, WebApiClientServiceImp>();
        }
    }
}