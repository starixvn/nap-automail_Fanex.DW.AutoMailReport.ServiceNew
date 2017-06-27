using Fanex.MailDeliver.Model;
using NegativeBalance.Business.Services;
using NUnit.Framework;

namespace NegativeBalance.UnitTests.Services
{
    [Category("NegativeBalance.UnitTests.Services MailDeliverServiceUnitTests")]
    public class MailDeliverServiceUnitTests
    {
        [Test]
        public void Send_MailToEmpty_SendMailUnSuccessfully()
        {
            // Arrange
            var mail = new MailRecord();
            var expected = false;

            // Act
            var service = new MailDeliverServiceImp();
            var result = service.Send(mail);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Send_Always_SendMailSuccessfully()
        {
            // Arrange
            var mail = new MailRecord
            {
                Body = "Negative Balance - Testing",
                MailTo = "robert.tran@starixsoft.com",
                Title = "Auto Generated Report Administrator",
                Subject = "Automail Report - 12bet - List of Members with Negative Balance By Match - 03/07/2017 09:21:06 AM"
            };

            var expected = true;

            // Act
            var service = new MailDeliverServiceImp();
            var result = service.Send(mail);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}