using MaxLossMonitoring.Business.Services;
using NUnit.Framework;
using SimpleInjector;

namespace MaxLossMonitoring.UnitTests.Services
{
    [Category("MaxLossMonitoring.UnitTests.Services MessageContentServiceUnitTests")]
    public class MessageContentServiceUnitTests
    {
        [Test]
        public void GenerateMessageContent_Always_ReturnRightContent()
        {
            // Arrange
            FanexDataRegisterService.ConfigureFanexData();
            var container = new Container();
            SimpleInjectorService.Register(container);

            // Action
            var dataService = container.GetInstance<DataService>();
            var service = container.GetInstance<MessageContentService>();
            var datasource = dataService.QueryMonitorListForLetstalk();

            foreach (var group in datasource)
            {
                var result = service.GenerateMessageContent(group);
            }

            // Assert
        }
    }
}