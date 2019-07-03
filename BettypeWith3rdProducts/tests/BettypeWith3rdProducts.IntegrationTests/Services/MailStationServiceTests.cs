using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;
using NUnit.Framework;

namespace BettypeWith3rdProducts.IntegrationTests.Services
{
    [Category("BettypeWith3rdProducts.IntegrationTests.Services MailStationServiceTests")]
    public class MailStationServiceTests
    {
        [Test]
        public void Send_Always_Successfully()
        {
            // Arrange
            var mail = new Mail
            {
                Body = "Bettype With 3rd Products is running Integration Tests",
                Subject = "Automail Report - Bettype With 3rd Products",
                Title = "Auto Generated Report Administrator",
                MailingListId = 90
            };

            // Act
            var service = new MailStationServiceImp();
            var result = service.Send(mail);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }
    }
}