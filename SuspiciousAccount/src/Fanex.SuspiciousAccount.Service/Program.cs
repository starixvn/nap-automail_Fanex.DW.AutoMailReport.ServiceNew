using Fanex.SuspiciousAccount.Business.Services;
using Fanex.SuspiciousAccount.Service;
using SimpleInjector;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    internal class Program
    {
        private static void Main()
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            RegisterService.RegisterSimpleInjector(container);
            container.Verify();

            

            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(container);
                config.Service<NotificationService>(s =>
                {
                    s.ConstructUsingSimpleInjector();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                config.SetServiceName(AppConfigService.Config.ServiceName);
                config.SetDisplayName(AppConfigService.Config.ServiceName);
                config.SetDescription(AppConfigService.Config.ServiceName);
            });
        }

       
    }
}