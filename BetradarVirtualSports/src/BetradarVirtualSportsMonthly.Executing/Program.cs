using System;
using Fanex.Service.Core;
using Newtonsoft.Json;
using SimpleInjector;

namespace BetradarVirtualSportsMonthly.Executing
{
    public static class Program
    {
        public static void Main()
        {
            FanexDataRegisterService.ConfigureFanexData();

            Container container = new Container();
            MonthlySimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute();
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}