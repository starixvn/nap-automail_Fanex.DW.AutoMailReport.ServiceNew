using BetradarVirtualSportsDaily.Business.Services;
using BetradarVirtualSportsDaily.Executing;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace BetradarVirtualSports.Executing
{
    public static class Program
    {
        public static void Main()
        {
            Container container = new Container();
            RegisterService.RegisterServices();
            DailySimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}