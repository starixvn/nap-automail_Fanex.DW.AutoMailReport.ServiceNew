using System;
using System.Configuration;
using System.IO;
using Fanex.Data;
using Fanex.Data.OldConnectionStringProvider;
using Fanex.Data.Repository;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.Framework;
using OfficeOpenXml;
using SimpleInjector;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.UnitTests.Services
{
    [Category("TopWinningAgent.UnitTests.Services MailServiceTests")]
    public class MailServiceTests
    {
        [Test]
        public void Execute_Always_ReturnRightContent()
        {
            // Arrange
            ConfigureFanexData();
            Container container = new Container();
            Register(container);
            container.Verify(VerificationOption.VerifyOnly);
            var expected = JobStatus.Success;

            // Action
            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute();
            var status = result.JobStatus;

            // Assert
            Assert.That(status, Is.EqualTo(expected));
        }

        internal static void ConfigureFanexData()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var storedProcedurePath = ConfigurationManager.AppSettings["StoredProcedurePath"];
            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider($@"~/{storedProcedurePath}", enableWatching: true)
                .Run();
        }

        internal static void Register(Container container)
        {
            container.Register(() => new ExcelPackage(new FileInfo(@"Templates\TopWinningTemplate.xlsx")));
            container.Register<DateRangeService, DateRangeServiceImp>();
            container.Register<DataSourceService, DataSourceServiceImp>();
            container.Register<ExcelService, ExcelServiceImp>();
            container.Register<ApplicationConfigurationService, ApplicationConfigurationServiceImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<LogService, LogServiceImp>();
            container.Register<FileService, FileServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<DistributedNotificationService, DistributedNotificationServiceImp>();
            container.Register<RepositoryService, RepositoryServiceImp>();
        }
    }
}