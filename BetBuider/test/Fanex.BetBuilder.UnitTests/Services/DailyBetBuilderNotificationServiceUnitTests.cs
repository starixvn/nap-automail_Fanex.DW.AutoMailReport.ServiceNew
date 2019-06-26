using System.IO;
using Fanex.BetBuilder.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector;

namespace Fanex.BetBuilder.UnitTests.Services
{
    [Category("Fanex.BetBuilder.UnitTests.Services.DailyBetBuilderNotificationServiceUnitTests")]
    public class DailyBetBuilderNotificationServiceUnitTests
    {
        private Container container;

        [SetUp]
        public void Init()
        {
            container = new Container();
            RegisterService.RegisterContainer(container);
        }

        [Test]
        public void Execute_Always_ReturnMailNoAttachement()
        {
            // Arrange
            var text = File.ReadAllText("Templates/MailResultNoAttachment.json");
            var expected = JsonConvert.DeserializeObject<Mail>(text);

            // Act
            var service = container.GetInstance<DailyBetBuilderNotificationService>();
            var mail = service.Execute();
            var result = mail.Mail;
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(!string.IsNullOrEmpty(result.Body));
        }
    }
}