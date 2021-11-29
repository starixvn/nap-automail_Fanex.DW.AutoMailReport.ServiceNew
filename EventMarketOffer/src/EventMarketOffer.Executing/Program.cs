using EventMarketOffer.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace EventMarketOffer.Executing
{
    public static class Program
    {
        public static void Main()
        {
            RegisterService.RegisterServices();
            var container = new Container();
            SimpleInjectorService.Register(container);

            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);

            Environment.Exit(0);
        }
    }
}