using Fanex.MarginChart.Executing.Services;
using Topshelf;

namespace Fanex.MarginChart.Executing
{
    internal class Program
    {
        private static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<MarginChartService>(s =>
                {
                    s.ConstructUsing(name => new MarginChartService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                //x.SetDescription("Fanex.MarginChart.Executing Host");
                //x.SetDisplayName("Fanex.MarginChart.Executing");
                //x.SetServiceName("Fanex.MarginChart.Executing");
            });
        }
    }
}