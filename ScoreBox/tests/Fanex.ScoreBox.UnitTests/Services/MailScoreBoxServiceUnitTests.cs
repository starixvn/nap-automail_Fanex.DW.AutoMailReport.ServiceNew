using Fanex.MailDelivery.Core.Models;
using Fanex.ScoreBox.Business.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace Fanex.ScoreBox.UnitTests.Services
{
    [Category("Fanex.ScoreBox.UnitTests.Services.MailScoreBoxServiceUnitTests")]
    public class MailScoreBoxServiceUnitTests
    {
        [Test]
        public void CreateMail_Daily_ReturnRightContent()
        {
            // Arrange
            var date = new DateTime(2019, 08, 12);
            var text = File.ReadAllText(@"Templates/MailDaily-expected.json");
            var expected = JsonConvert.DeserializeObject<Mail>(text);

            // Act
            var service = new MailScoreBoxServiceImp();
            var result = service.CreateMail(true, date, date, @"Templates/DailyScoreBox.xlsx", false);            

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }

        [Test]
        public void CreateMail_Monthly_ReturnRightContent()
        {
            // Arrange
            var date = new DateTime(2019, 08, 12);
            var text = File.ReadAllText(@"Templates/MailMonthly-expected.json");
            var expected = JsonConvert.DeserializeObject<Mail>(text);

            // Act
            var service = new MailScoreBoxServiceImp();
            var result = service.CreateMail(false, date, date, @"Templates/DailyScoreBox.xlsx", false);
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }
    }
}