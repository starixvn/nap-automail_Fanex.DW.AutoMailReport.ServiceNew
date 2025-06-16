using Fanex.DailyDHWL.Executing.Services;
using Topshelf;

namespace Fanex.DailyDHWL.Executing
{
    internal class Program
    {
        private static void Main()
        {
            //RegisterService.RegisterServices();
            //Container container = new Container();
            //RegisterService.RegisterContainer(container);
            //container.Verify(VerificationOption.VerifyOnly);

            //var service = container.GetInstance<DailyDHWLNotificationService>();
            //var jobMessage = service.Execute();
            //var json = JsonConvert.SerializeObject(jobMessage);

            //Console.Write(json);
            //Environment.Exit(0);

            HostFactory.Run(x =>
            {
                x.Service<DHWLService>(s =>
                {
                    s.ConstructUsing(name => new DHWLService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetDescription("Fanex.DailyDHWL.Executing Host");
                x.SetDisplayName("Fanex.DailyDHWL.Executing");
                x.SetServiceName("Fanex.DailyDHWL.Executing");
            });
        }
    }
}