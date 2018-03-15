using System;
using Fanex.UpdateExchangeRate.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;

namespace Fanex.UpdateExchangeRate.Executing
{
    public static class Program
    {
        public static void Main()
        {
            FanexDataRegisterService.ConfigureFanexData();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
#pragma warning disable S2228 // Console logging should not be used
            Console.WriteLine(json);
#pragma warning restore S2228 // Console logging should not be used
            Environment.Exit(0);
        }
    }
}