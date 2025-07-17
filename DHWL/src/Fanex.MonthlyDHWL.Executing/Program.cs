using Fanex.MonthlyDHWL.Executing.Services;
using Topshelf;

namespace Fanex.MonthlyDHWL.Executing
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

                x.SetDescription("Fanex.MonthlyDHWL.Executing Host");
                x.SetDisplayName("Fanex.MonthlyDHWL.Executing");
                x.SetServiceName("Fanex.MonthlyDHWL.Executing");
            });
        }
    }
}