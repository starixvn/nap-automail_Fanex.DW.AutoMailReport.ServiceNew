using Fanex.ESportModelingData.Business.Models;
using Fanex.ESportModelingData.Business.Services;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.ExportBaseServiceUnitTests")]
    public class ExportBaseServiceUnitTests
    {
        [Test]
        public void ExporDimension_MethodExecute_ShouldBeCalled()
        {
            // Arrange
            var csvService = Substitute.For<CsvService>();
            var text = File.ReadAllText("Templates/RawModelingDataModel.json");
            var status = JsonConvert.DeserializeObject<List<RawModelingDataModel>>(text);

            // Act
            var service = new ExportBaseServiceImp(csvService);
            status.GroupBy(g => new { g.Status })
            .ToList()
            .ForEach(statusGroup =>
            {
                service.ExporDimension(string.Empty, string.Empty, string.Empty, statusGroup);
            });

            // Assert
            csvService.Received().Execute(Arg.Any<List<BetCountModel>>(), Arg.Any<string>());
            csvService.Received().Execute(Arg.Any<List<MemberTurnoverModel>>(), Arg.Any<string>());
            csvService.Received().Execute(Arg.Any<List<MemberWinLossModel>>(), Arg.Any<string>());
            csvService.Received().Execute(Arg.Any<List<VirtualrateSettingModel>>(), Arg.Any<string>());
        }

        [Test]
        [TestCase(@"\")]
        [TestCase(@"/")]
        [TestCase(@"|")]
        [TestCase(@"?")]
        [TestCase(@"*")]
        [TestCase(@"<")]
        [TestCase(@">")]
        [TestCase(@":")]
        public void Replace_Alway_ReturnRemoveSpecialCharactor(string input)
        {
            // Arrange
            var csvService = Substitute.For<CsvService>();
            var expect = " ";

            // Act
            var service = new ExportBaseServiceImp(csvService);
            var result = service.Replace(input);

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }
    }
}