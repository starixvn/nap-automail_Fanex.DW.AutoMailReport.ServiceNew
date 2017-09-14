using System;
using System.IO;
using MaxLossControl.CustomerMovement.Business.Services;
using NSubstitute;
using NUnit.Framework;

namespace MaxLossControl.CustomerMovement.UnitTests.Services
{
    [Category("MaxLossControl.CustomerMovement.UnitTests.Services FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [SetUp]
        public void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void ReadContent_Always_ReturnRightContent()
        {
            // Arrange
            var data = new FakeDataService().Query();
            var fromDate = new DateTime(2017, 08, 29);
            var toDate = new DateTime(2017, 08, 31);
            var date = Substitute.For<DateService>();
            date.FromDate.Returns(fromDate);
            date.ToDate.Returns(toDate);

            // Act
            var service = new FileServiceImp();
            var result = service.ReadContent(data, date);

            // Assert
            var expected = File.ReadAllText(@"Templates\FileService-ReadContent-Expected.txt");
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}