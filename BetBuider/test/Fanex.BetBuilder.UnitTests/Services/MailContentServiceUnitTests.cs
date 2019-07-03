using System;
using System.IO;
using Fanex.BetBuilder.Business.Services;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.MailContentServiceUnitTests")]
    public class MailContentServiceUnitTests
    {
        [Test]
        public void GenerateDailyMailContent_NoData_ReturnRightContent()
        {
            // Arrange
            var expected = File.ReadAllText("Templates/DailyEmailContentNoData.txt");
            var winlostDate = new DateTime(2019, 05, 01);

            // Act
            var service = new MailContentServiceImp();
            var result = service.GenerateDailyMailContent(winlostDate, true);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GenerateDailyMailContent_HasData_ReturnRightContent()
        {
            // Arrange
            var expected = File.ReadAllText("Templates/DailyMailContentHasData.txt");
            var winlostDate = new DateTime(2019, 05, 01);

            // Act
            var service = new MailContentServiceImp();
            var result = service.GenerateDailyMailContent(winlostDate, false);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GenerateMonthlyMailContent_NoData_ReturnRightContent()
        {
            // Arrange
            var expected = File.ReadAllText("Templates/MonthlyEmailContentNoData.txt");
            var fromdate = new DateTime(2019, 05, 01);
            var toate = new DateTime(2019, 05, 31);

            // Act
            var service = new MailContentServiceImp();
            var result = service.GenerateMonthlyMailContent(fromdate, toate, true);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GenerateMonthlyMailContent_HasData_ReturnRightContent()
        {
            // Arrange
            var expected = File.ReadAllText("Templates/MonthlyMailContentHasData.txt");
            var fromdate = new DateTime(2019, 05, 01);
            var toate = new DateTime(2019, 05, 31);

            // Act
            var service = new MailContentServiceImp();
            var result = service.GenerateMonthlyMailContent(fromdate, toate, false);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}