using System;
using System.Collections.Generic;
using System.IO;
using Fanex.Service.Core;
using MaxLossMonitoring.Business.Models;
using MaxLossMonitoring.Business.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using OfficeOpenXml;
using SimpleInjector;

namespace MaxLossMonitoring.UnitTests.Services
{
    [Category("MaxLossMonitoring.UnitTests.Services ExcelServiceUnitTests")]
    public class ExcelServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void ExportExcel()
        {
            // Arrange
            var appConfig = Substitute.For<ApplicationConfigurationService>();
            appConfig.Configurations.Returns(new AppConfig { ExportDataDirectory = "\\\\10.70.70.242\\www\\Olympus\\Fanex.Excel\\ListofSMAReachingAllowableMaxLoss", ExcelPasswordProtected = string.Empty });
            var data = JsonConvert.DeserializeObject<Tuple<List<MonitorList>, List<MonitorListRaw>>>(File.ReadAllText("Templates/ExpectedQueryMonitorListForEmail.json"));
            var container = new Container();
            SimpleInjectorService.Register(container);
            var excel = container.GetInstance<ExcelPackage>();
            SystemTime.Set(new DateTime(2018, 03, 02));
            var expected = @"\\10.70.70.242\www\Olympus\Fanex.Excel\ListofSMAReachingAllowableMaxLoss\2018\03\List_Of_SMA_Reaching_Allowable_Max_Loss_For_2018_03_02.xlsx";

            // Action
            var service = new ExcelServiceImp(appConfig, excel);
            var result = service.Export(data).Save();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}