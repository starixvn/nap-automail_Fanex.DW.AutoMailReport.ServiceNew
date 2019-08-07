using Fanex.CN88DailyWL.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace Fanex.CN88DailyWL.Executing
{
    internal static class Program
    {
        private static void Main()
        {
            RegisterService.RegisterServices();
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<CN88NotifycationService>();
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}