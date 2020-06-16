using Fanex.ESportModelingData.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.ExportLevelFactoryUnitTests")]
    public class ExportLevelFactoryUnitTests
    {
        [Test]
        public void Execute_Always_CallRightDataLevel1()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportBasketballLevel1ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.ExecuteESports()[1].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel2()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportESportLevel2ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.ExecuteESports()[2].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel3()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportESportLevel3ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.ExecuteESports()[3].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel4()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportESportLevel4ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.ExecuteESports()[4].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }
    }
}