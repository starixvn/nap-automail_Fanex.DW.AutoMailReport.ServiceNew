using System;
using System.Configuration;
using System.IO;
using Fanex.ServiceMonitoring.Service;
using SimpleInjector;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    internal class Program
    {
        private static Container container;

        private static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            RegisterService.RegisterServices();

            RegisterContainer();
            SetupTopShelf();
        }

        private static void RegisterContainer()
        {
            container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify();
        }

        private static void SetupTopShelf()
        {
            var serviceName = ConfigurationManager.AppSettings["ServiceName"];
            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(container);

                config.Service<ServiceMonitoring>(s =>
                {
                    s.ConstructUsingSimpleInjector();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                config.SetServiceName(serviceName);
                config.SetDisplayName(serviceName);
                config.SetDescription(serviceName);
            });
        }
    }
}