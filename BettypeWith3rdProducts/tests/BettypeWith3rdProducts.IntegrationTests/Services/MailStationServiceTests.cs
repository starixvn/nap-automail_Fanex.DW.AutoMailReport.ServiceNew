using BettypeWith3rdProducts.Business.Services;
using Fanex.MailDelivery.Core.Models;
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
                Body = "Body Test",
                Subject = "Subject Test",
                Title = "Title Test",
                MailingListId = 90
            };

            var expected = true;

            // Act
            var service = new MailStationServiceImp();
            var result = service.Send(mail);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}