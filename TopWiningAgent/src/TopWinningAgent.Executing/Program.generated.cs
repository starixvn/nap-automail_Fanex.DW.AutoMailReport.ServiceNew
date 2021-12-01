using Newtonsoft.Json;
using SimpleInjector;
using System;
using TopWinningAgent.Business.Services;

namespace TopWinningAgent.Executing
{
    public static class Program
    {
        public static void Main()
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            SimpleInjectorService.Register(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationService>();
            var result = service.Execute();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Environment.Exit(0);
        }
    }
}