using System;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using FastMarketWeeklyPerformance.Business.Services;
using NUnit.Framework;
using OfficeOpenXml;
using SimpleInjector;

namespace FastMarketWeeklyPerformance.IntegrationTests
{
    [Category("FastMarketWeeklyPerformance.IntegrationTests ProgramIntegrationTests")]
    public class ProgramIntegrationTests
    {
        private void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($"~/App_Data/Local", enableWatching: true)
                .Run();
        }

        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\Template.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailStationService, MailStationServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<LogService, LogServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
        }

        [Test]
        public void RunAutoMail_Always_Successfully()
        {
            // Arrange
            var container = new Container();
            ConfigureFanexData();
            Register(container);
            var expected = JobStatus.Success;

            // Act
            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute().JobStatus;

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}