using Fanex.DailyDHWL.Executing.Services;
using Topshelf;

namespace Fanex.DailyDHWL.Executing
{
    internal class Program
    {
        private static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<DHWLService>(s =>
                {
                    s.ConstructUsing(name => new DHWLService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
            });
        }
    }
}