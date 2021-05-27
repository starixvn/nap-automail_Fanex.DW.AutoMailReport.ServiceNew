using Fanex.CWLGamePayoutAlert.Service;
using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

namespace Fanex.Dangerous789YCustomer.Service
{
    internal class Program
    {
        private static void Main()
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(container);
                config.Service<NotificationService>(s =>
                {
                    s.ConstructUsingSimpleInjector();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                config.SetServiceName("Fanex.CWLGamePayoutAlert.Service");
                config.SetDisplayName("Fanex.CWLGamePayoutAlert.Service");
                config.SetDescription("Fanex.CWLGamePayoutAlert.Service");
            });
        }
    }
}