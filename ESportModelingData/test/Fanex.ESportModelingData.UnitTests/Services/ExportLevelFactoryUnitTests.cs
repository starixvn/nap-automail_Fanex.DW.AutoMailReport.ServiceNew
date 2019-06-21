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
            var expect = typeof(ExportLevel1ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.Execute()[1].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel2()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportLevel2ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.Execute()[2].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel3()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportLevel3ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.Execute()[3].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }

        [Test]
        public void Execute_Always_CallRightDataLevel4()
        {
            // Arrange
            var exportBaseService = Substitute.For<ExportBaseService>();
            var expect = typeof(ExportLevel4ServiceImp);

            // Act
            var service = new ExportLevelFactoryImp(exportBaseService);
            var result = service.Execute()[4].GetType();

            // Assert
            Assert.That(result, Is.EqualTo(expect));
        }
    }
}