using BetradarVirtualSportsMonthly.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace BetradarVirtualSportsMonthly.Executing
{
    public static class Program
    {
        public static void Main()
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            MonthlySimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}