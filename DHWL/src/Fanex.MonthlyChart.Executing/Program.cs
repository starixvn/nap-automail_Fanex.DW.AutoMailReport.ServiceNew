using Fanex.MonthlyChart.Executing.Services;
using Topshelf;

namespace Fanex.MonthlyChart.Executing
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

                x.SetDescription("Fanex.MonthlyChart.Executing Host");
                x.SetDisplayName("Fanex.MonthlyChart.Executing");
                x.SetServiceName("Fanex.MonthlyChart.Executing");
            });
        }
    }
}