using System;
using System.IO;
using Fanex.BetBuilder.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.MailServiceUnitTests")]
    public class MailServiceUnitTests
    {
        [Test]
        public void SendMail_NoData_ReturnRightContent()
        {
            // Arrange
            var winlostDate = new DateTime(2019, 05, 01);
            var mailContentService = Substitute.For<MailContentService>();
            var datetimeService = Substitute.For<DateTimeService>();
            var param = Substitute.For<CriteriaService>();
            param.WinlostDate.Returns(winlostDate);
            var text = File.ReadAllText("Templates/DailyEmailContentNoData.txt");
            mailContentService.GenerateDailyMailContent(winlostDate, true).Returns(text);
            text = File.ReadAllText("Templates/MailResultNoAttachment.json");
            var expected = JsonConvert.DeserializeObject<Mail>(text);

            // Act
            var service = new MailServiceImp(mailContentService, param, datetimeService);
            var result = service.SendDailyMail("fileName", true);

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }
    }
}