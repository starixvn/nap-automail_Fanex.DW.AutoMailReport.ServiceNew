using System;
using System.Collections.Generic;
using System.IO;
using Fanex.Data.Repository;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.ExportServiceUnitTests")]
    public class ExportServiceUnitTests
    {
        private Container container;

        [SetUp]
        public void RegisterContainer()
        {
            container = new Container();
            container.Register<Log, LogImp>();
            container.Register<IDynamicRepository, DynamicRepository>();
            container.Register<RepositoryService, RepositoryServiceImp>();
            container.Register<DateTimeService, DateTimeServiceImp>();
            container.Register<ModelingDataNotificationService, ModelingDataNotificationServiceImp>();
            container.Register<ExportService, ExportServiceImp>();
            container.Register<CsvService, CsvServiceImp>();
            container.Register<ExportBaseService, ExportBaseServiceImp>();
            container.Register<ExportLevelFactory, ExportLevelFactoryImp>();
            container.Register<MailContentService, MailContentServiceImp>();
            container.Register<MailService, MailServiceImp>();
            container.Register<ParamService, ParamServiceImp>();
            container.Verify(VerificationOption.VerifyOnly);
        }

        [Test]
        public void Execute_Always_FactoryShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var rawData = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);
            var dateStart = new DateTime(2019, 06, 17);
            var dateEnd = new DateTime(2019, 06, 23);
            var expected = @"D:\E_SportsWeeklyModelingData\2019\06\E-Sports Weekly Modeling Data From 23-06-2019 To 23-06-2019.zip";

            // Act
            var service = container.GetInstance<ExportService>();
            var result = service.Execute(dateEnd, dateEnd, rawData);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}