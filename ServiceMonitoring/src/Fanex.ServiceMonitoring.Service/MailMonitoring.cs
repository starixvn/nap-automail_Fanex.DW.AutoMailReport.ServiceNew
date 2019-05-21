using System;
using System.Timers;
using Fanex.ServiceMonitoring.Services;
using ONEbook.UM.Services;

namespace Fanex.ServiceMonitoring.Service
{
    public class MailMonitoring
    {
        private const int Milliseconds = 1000;

        private readonly IUMControlAdapter umcontrolAdapter;
        private readonly MessageService messageService;
        private readonly Log log;

        private Timer timer;
        private bool isOnProcessing;

        public MailMonitoring(IUMControlAdapter umcontrolAdapter, MessageService messageService, Log log)
        {
            this.umcontrolAdapter = umcontrolAdapter;
            this.messageService = messageService;
            this.log = log;

            Register();
        }

        public void Start()
        {
            timer.Start();
        }

        private void Register()
        {
            timer = new Timer();
            timer.Interval = AppConfigService.AppConfig.IntervalInSecondMailMonitor * Milliseconds;
            timer.Elapsed += (sender, e) =>
            {
                if (!isOnProcessing)
                {
                    Execute();
                }
            };
        }

        private void Execute()
        {
            try
            {
                if (umcontrolAdapter.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    return;
                }

                isOnProcessing = true;
                messageService.SendMessage();
                isOnProcessing = false;
            }
            catch (Exception exception)
            {
                isOnProcessing = false;
                log.Log(exception);
            }
        }
    }
}