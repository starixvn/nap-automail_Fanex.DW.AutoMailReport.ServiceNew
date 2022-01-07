using System;
using Newtonsoft.Json;
using SimpleInjector;
using TOControl.Notification.Business.Enums;
using TOControl.Notification.Business.Services;

namespace TOControl.CustomerMovement.Executing
{
    public static class Program
    {
        public static void Main()
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationMovementService>();
            var notifiedByFunction = NotifiedByFunction.TOControl;
            var result = service.Execute(notifiedByFunction);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}