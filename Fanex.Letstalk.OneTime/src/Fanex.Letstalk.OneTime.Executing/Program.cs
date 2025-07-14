using Fanex.Letstalk.OneTime.Executing.Services;
using Topshelf;

namespace Fanex.Letstalk.OneTime.Executing
{
    internal class Program
    {
        private static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<OneTimeService>(s =>
                {
                    s.ConstructUsing(name => new OneTimeService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetDescription("Fanex.Letstalk.OneTime.Executing Host");
                x.SetDisplayName("Fanex.Letstalk.OneTime.Executing");
                x.SetServiceName("Fanex.Letstalk.OneTime.Executing");
            });
        }
    }
}