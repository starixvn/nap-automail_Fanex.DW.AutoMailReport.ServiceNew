using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

namespace Fanex.AutoDelFiles.Service
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

                config.SetServiceName("Fanex.AutoDelFiles.Service");
                config.SetDisplayName("Fanex.AutoDelFiles.Service");
                config.SetDescription("Fanex.AutoDelFiles.Service");
            });
        }
    }
}