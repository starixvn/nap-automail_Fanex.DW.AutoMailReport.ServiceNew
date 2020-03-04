using SimpleInjector;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    internal class Program
    {
        private static void Main()
        {
            Container container = new Container();
            ConfigureContainer(container);
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

                config.SetServiceName("My.Cool.Service");
                config.SetDisplayName("My Cool Service");
                config.SetDescription("What is my purpose?");
            });
        }

        private static void ConfigureContainer(Container container)
        {
            container.Register<NotificationService>();
        }
    }
}