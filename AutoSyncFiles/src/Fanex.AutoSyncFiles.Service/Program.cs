using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

namespace Fanex.AutoSyncFiles.Service
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

                config.SetServiceName("Fanex.AutoSyncFiles.Service");
                config.SetDisplayName("Fanex.AutoSyncFiles.Service");
                config.SetDescription("Fanex.AutoSyncFiles.Service");
            });
        }
    }
}