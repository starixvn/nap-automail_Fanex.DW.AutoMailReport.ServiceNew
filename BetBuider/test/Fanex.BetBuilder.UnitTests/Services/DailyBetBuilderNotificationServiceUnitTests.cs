using Fanex.BetBuilder.Business.Services;
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

            // Act
            var service = container.GetInstance<DailyBetBuilderNotificationService>();
            var mail = service.Execute();
            var result = mail.Mail;
            JsonConvert.SerializeObject(result, Formatting.Indented);

            // Assert
            Assert.That(!string.IsNullOrEmpty(result.Body));
        }
    }
}