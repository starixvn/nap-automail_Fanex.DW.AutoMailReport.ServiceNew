using System.Configuration;
using System.IO;
using BettypeWith3rdProducts.Business.Repositories;
using BettypeWith3rdProducts.Business.Services;
using BettypeWith3rdProducts.BusinessSubstitute;
using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;

namespace BettypeWith3rdProducts.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeServiceProvider, DateRangeServiceProviderImp>();
            container.Register<ParameterService, ParameterServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<SportSplitterService, SportSplitterServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ExcelHeaderService, ExcelHeaderServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailContentBuilder, MailContentBuilderImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<LogService, LogServiceImp>();

            if (ConfigurationManager.AppSettings["IsFakeData"] == "true")
            {
                container.Register<DataService, DataServiceSubstitute>();
            }
            else
            {
                container.Register<DataService, DataServiceImp>();
            }
        }
    }
}