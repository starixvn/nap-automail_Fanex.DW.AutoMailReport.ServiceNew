using Fanex.LicenseeFinanceCode.Business.Service;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace Fanex.LicenseeFinanceCode.Executing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RegisterService.RegisterServices();
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<NotificationService>();
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}