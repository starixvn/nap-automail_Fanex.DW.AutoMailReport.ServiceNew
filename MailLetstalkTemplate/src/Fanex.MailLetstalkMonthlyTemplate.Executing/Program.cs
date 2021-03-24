using Fanex.MailLetstalkTemplate.Business.Services;
using Newtonsoft.Json;
using SimpleInjector;
using System;

namespace Fanex.MailLetstalkTemplate.Executing
{
    internal class Program
    {
        private static void Main()
        {
            var s = new DateRangeMonthlyServiceImp();
            var w = new DateRangeDailyServiceImp();

            RegisterService.RegisterServices();
            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<MailLetstalkTemplateNotification>();
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}