using System;
using System.Collections.Generic;
using System.IO;
using Fanex.ESportModelingData.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.ESportModelingData.UnitTests.Services
{
    [Category("Fanex.ESportModelingData.UnitTests.Services.MailServiceUnitTests")]
    public class MailServiceUnitTests
    {
        [Test]
        public void CreateMail_Always_ReturnRightData()
        {
            // Arrange
            var mailContent = File.ReadAllText("Templates/EmailContent.txt");
            var mailContentService = Substitute.For<MailContentService>();
            mailContentService.GenerateMailContent(Arg.Any<List<string>>()).Returns(mailContent);
            var dateStart = new DateTime(2019, 06, 17);
            var dateEnd = new DateTime(2019, 06, 23);
            var text = File.ReadAllText("Templates/CreateMail-expected.json");
            var expected = JsonConvert.DeserializeObject<Mail>(text);

            // Act
            var service = new MailServiceImp(mailContentService);
            var result = service.CreateMail(dateStart, dateEnd, new List<string> { "FileName" });

            // Assert
            Assert.That(result, NUnit.DeepObjectCompare.Is.DeepEqualTo(expected));
        }
    }
}