using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Fanex.ESportModelingData.UnitTests.Models;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.ExportLevelServiceUnitTests")]
    public class ExportLevelServiceUnitTests
    {
        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var text = File.ReadAllText("Templates/ESportModelingDataSettings.json");
            AppConfigService.AppConfig = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
        }

        [Test]
        public void Execute_Level1_MethodShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var status = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);
            var exportBaseService = Substitute.For<ExportBaseService>();
            exportBaseService.Replace(Arg.Any<string>()).Returns(" ");

            // Act
            var level1Service = new ExportBasketballLevel1ServiceImp(exportBaseService);
            status.GroupBy(g => new GroupModel { Level = g.Level, Sport = g.Sport })
            .ToList()
            .ForEach(level =>
           {
               level1Service.Execute(level);
           });

            // Assert
            exportBaseService.Received().ExporDimension(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IGrouping<dynamic, RawModelingDataModel>>());
        }

        [Test]
        public void Execute_Level2_MethodShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var status = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);
            var exportBaseService = Substitute.For<ExportBaseService>();
            exportBaseService.Replace(Arg.Any<string>()).Returns(" ");

            // Act
            var level2Service = new ExportESportLevel2ServiceImp(exportBaseService);
            status
                .Where(l => l.Level == 2)
                .GroupBy(g => new GroupModel { Level = g.Level, Sport = g.Sport })
                .ToList()
                .ForEach(level =>
                {
                    level2Service.Execute(level);
                });

            // Assert
            exportBaseService.Received().ExporDimension(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IGrouping<dynamic, RawModelingDataModel>>());
        }

        [Test]
        public void Execute_Level3_MethodShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var status = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);
            var exportBaseService = Substitute.For<ExportBaseService>();
            exportBaseService.Replace(Arg.Any<string>()).Returns(" ");

            // Act
            var level3Service = new ExportESportLevel3ServiceImp(exportBaseService);
            status
                .Where(l => l.Level == 3)
                .GroupBy(g => new GroupModel { Level = g.Level, Sport = g.Sport })
                .ToList()
                .ForEach(level =>
                {
                    level3Service.Execute(level);
                });

            // Assert
            exportBaseService.Received().ExporDimension(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IGrouping<dynamic, RawModelingDataModel>>());
        }

        [Test]
        public void Execute_Level4_MethodShouldBeCalled()
        {
            // Arrange
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var status = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);
            var exportBaseService = Substitute.For<ExportBaseService>();
            exportBaseService.Replace(Arg.Any<string>()).Returns(" ");

            // Act
            var level4Service = new ExportESportLevel4ServiceImp(exportBaseService);
            status
                .Where(l => l.Level == 4)
                .GroupBy(g => new GroupModel { Level = g.Level, Sport = g.Sport })
                .ToList()
                .ForEach(level =>
                {
                    level4Service.Execute(level);
                });

            // Assert
            exportBaseService.Received().ExporDimension(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IGrouping<dynamic, RawModelingDataModel>>());
        }
    }
}