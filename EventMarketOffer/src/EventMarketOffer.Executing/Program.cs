using System;
using Fanex.Service.Core;
using Newtonsoft.Json;
using SimpleInjector;

namespace EventMarketOffer.Executing
{
    public static class Program
    {
        public static void Main()
        {
            FanexDataRegisterService.ConfigureFanexData();
            var container = new Container();
            SimpleInjectorService.Register(container);

            var service = container.GetInstance<DistributedNotificationService>();
            var result = service.Execute();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
#pragma warning disable S2228 // Console logging should not be used
            Console.WriteLine(json);
#pragma warning restore S2228 // Console logging should not be used
            Environment.Exit(0);
        }
    }
}