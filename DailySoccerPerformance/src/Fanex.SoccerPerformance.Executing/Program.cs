using Fanex.SoccerPerformance.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace Fanex.SoccerPerformance.Executing
{
    internal static class Program
    {
        private static void Main()
        {
            RegisterService.RegisterServices();
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<SoccerPerformanceNotification>();
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}
