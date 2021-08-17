using System;
using System.Collections.Generic;
using System.Threading;

namespace Fanex.AutoDelFiles.Business.Service
{
    public interface DelFileNotificationService
    {
        void Execute();
    }

    public class DelFileNotificationServiceImp : DelFileNotificationService
    {
        private const int DONE = 0;
        private const int RUNNING = 1;
        private int state = DONE;
        private readonly Log log;
        private readonly FactoryDelFileService delFileService;

        public DelFileNotificationServiceImp(Log log, FactoryDelFileService delFileService)
        {
            this.log = log;
            this.delFileService = delFileService;
        }

        private readonly List<string> errs = new List<string>
        {
            "An unexpected network error occurred",
            "Error when connect to remote share. networkName"
        };

        public void Execute()
        {
            try
            {
                AppConfigService.AppConfig.Paths.ForEach(path =>
                {
                    if (Interlocked.Exchange(ref state, RUNNING) == DONE)
                    {
                        Console.WriteLine($"Execute at {DateTime.Now}");
                        delFileService
                            .DelFiles(path)
                            .Execute(path);

                        Interlocked.Exchange(ref state, DONE);
                    }
                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                errs.ForEach(err =>
                {
                    if (!msg.Contains(msg))
                    {
                        log.Log(ex);
                    }
                });
            }
            finally
            {
                AppConfigService.IsProcessing = false;
            }
        }
    }
}