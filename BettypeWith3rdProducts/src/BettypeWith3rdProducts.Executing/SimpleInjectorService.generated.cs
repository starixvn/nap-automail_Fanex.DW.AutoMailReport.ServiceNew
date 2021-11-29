using BettypeWith3rdProducts.Business.Services;
using Fanex.Data.Repository;
using OfficeOpenXml;
using SimpleInjector;
using System.IO;

namespace BettypeWith3rdProducts.Executing
{
    internal static class SimpleInjectorService
    {
        internal static void Register(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();

            container.Register<SportSplitterService, SportSplitterServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ExcelHeaderService, ExcelHeaderServiceImp>();
            container.Register<FileService, FileServiceImp>();

            container.Register<MailContentBuilder, MailContentBuilderImp>();
            container.Register<MailService, MailServiceImp>();

            container.Register<RawDataSourceService, RawDataSourceServiceImp>();

            container.Register<DataService, DataServiceImp>();
            container.Register<NotificationService, NotificationServiceImp>();
            container.Register<RepositoryBettypeProductService, RepositoryServiceImp>();
            container.Register<Log, LogImp>();
        }
    }
}