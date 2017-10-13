using System;
using Newtonsoft.Json;
using SimpleInjector;
using TOControl.Notification.Business.Enums;
using TOControl.Notification.Business.Services;

namespace LossMonitoring.CustomerMovement.Executing
{
    public static class Program
    {
        public static void Main()
        {
            FanexDataRegisterService.ConfigureFanexData();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationMovementService>();
            var notifiedByFunction = NotifiedByFunction.LossMonitoring;
            var result = service.Execute(notifiedByFunction);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
#pragma warning disable S2228 // Console logging should not be used
            Console.WriteLine(json);
#pragma warning restore S2228 // Console logging should not be used
            Environment.Exit(0);
        }
    }
}