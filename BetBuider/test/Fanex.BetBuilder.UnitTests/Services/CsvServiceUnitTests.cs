using System;
using System.Collections.Generic;
using System.IO;
using Fanex.BetBuilder.Business.Models;
using Fanex.BetBuilder.Business.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.CsvServiceUnitTests")]
    public class CsvServiceUnitTests
    {
        [SetUp]
        public void Init()
        {
            var text = File.ReadAllText("Templates/DailyBetBuilderSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        [Test]
        public void ExportCsv_Always_Success()
        {
            // Arrange
            var text = File.ReadAllText("Templates/DailyDataModel.json");
            var dataModel = JsonConvert.DeserializeObject<List<DailyDataModel>>(text);
            var param = Substitute.For<CriteriaService>();
            param.WinlostDate.Returns(new DateTime(2019, 05, 01));
            var expected = @"\\10.18.200.120\www\Olympus\Fanex.Excel\BetBuilderDaily\2019\05\Bet_Builder_Daily_Report_20190501.zip";

            // Act
            var service = new CsvServiceImp(param);
            var result = service.ExportCsv(dataModel);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ExportCsv_File_Exists()
        {
            // Arrange
            var text = File.ReadAllText("Templates/DailyDataModel.json");
            var dataModel = JsonConvert.DeserializeObject<List<DailyDataModel>>(text);
            var param = Substitute.For<CriteriaService>();
            param.WinlostDate.Returns(new DateTime(2019, 05, 01));

            // Act
            var service = new CsvServiceImp(param);
            var path = service.ExportCsv(dataModel);
            
            // Assert
            Assert.That(File.Exists(path), Is.EqualTo(true));
        }
    }
}