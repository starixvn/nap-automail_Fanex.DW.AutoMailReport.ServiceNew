using Fanex.MailDelivery.Core.Models;
using NUnit.Framework;

namespace Fanex.Service.Core.UnitTests
{
    [Category("Fanex.Service.Core.UnitTests MailStationServiceTests")]
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

            var expected = true;

            // Act
            var service = new MailStationServiceImp();
            var result = service.Send(mail);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}