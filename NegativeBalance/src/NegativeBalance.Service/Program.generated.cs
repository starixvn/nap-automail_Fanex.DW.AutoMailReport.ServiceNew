using NegativeBalance.Service;
using SimpleInjector;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    public static class Program
    {
        public static void Main()
        {
            Container container = new Container();
            NegativeBalanceInjector.Register(container);
            container.Verify(VerificationOption.VerifyOnly);
            FanexDataRegisterService.ConfigureFanexData();

            HostFactory.Run(config =>
            {
                config.UseSimpleInjector(container);
                config.Service<NegativeBalanceService>(s =>
                {
                    s.ConstructUsingSimpleInjector();
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                config.SetServiceName("Fanex.NegativeBalance.Service");
                config.SetDisplayName("Fanex.NegativeBalance.Service");
                config.SetDescription("Notify email to users when members reach a negative balance.");
            });
        }
    }
}