using System;
using System.IO;
using Fanex.Service.Core;
using NUnit.Framework;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.UnitTests.Services
{
    [Category("TopWinningAgent.UnitTests.Services FileServiceUnitTests")]
    public class FileServiceUnitTests
    {
        [Test]
        public void ReadContent_Always_ReturnRightContent()
        {
            // Arrange
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            SystemTime.Set(new DateTime(2018, 02, 28));
            var dateRangeService = new DateRangeServiceImp();
            var expected = File.ReadAllText("Templates/EmailContentResult.txt");

            // Action
            var service = new FileServiceImp(dateRangeService);
            var result = service.ReadContent();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}