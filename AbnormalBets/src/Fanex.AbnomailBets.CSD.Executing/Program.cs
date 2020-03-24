using Fanex.AbnormalBets.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace Fanex.AbnomailBets.CSD.Executing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RegisterService.RegisterServices();
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<BetCSDNotificationService>();
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}