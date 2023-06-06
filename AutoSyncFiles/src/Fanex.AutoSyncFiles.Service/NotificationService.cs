using Fanex.AutoSyncFiles.Business.Services;
using System;

namespace Fanex.AutoSyncFiles.Service
{
    using System.Timers;
    using Topshelf;

    public class NotificationService
    {
        private Timer timer;

        private readonly SyncFilesService syncFilesService;

        private static bool running;

        public NotificationService()
        {
            var container = RegisterService.Container;
            syncFilesService = container.GetInstance<SyncFilesService>();
        }

        public bool Start()
        {
            timer?.Dispose();

            var interval = AppConfigService.AppConfig.TimerPeriodInSecond * 1000;
            timer = new Timer(interval);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;

            return true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (running)
            {
                return;
            }

            running = true;
            syncFilesService.Execute();
            var log = $"Fanex.AutoSyncFiles.Service run at {DateTime.Now}{Environment.NewLine}";
            Console.Out.WriteLine(log);
            running = false;
        }

        public bool Stop()
        {
            timer?.Stop();
            timer?.Dispose();

            return true;
        }
    }
}