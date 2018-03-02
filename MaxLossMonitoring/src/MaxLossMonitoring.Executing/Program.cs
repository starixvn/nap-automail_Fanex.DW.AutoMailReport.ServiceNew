using System;
using MaxLossMonitoring.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;

namespace MaxLossMonitoring.Executing
{
    public static class Program
    {
        public static void Main()
        {
            FanexDataRegisterService.ConfigureFanexData();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<DistributedNotificationLossMonitoringService>();
            var result = service.Execute();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}