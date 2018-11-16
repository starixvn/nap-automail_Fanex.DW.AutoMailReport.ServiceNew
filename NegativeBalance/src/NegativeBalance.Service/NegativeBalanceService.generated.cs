using System;
using System.Timers;
using Fanex.Service.Core;
using NegativeBalance.Business.Services;
using ONEbook.UM.Services;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    public class NegativeBalanceService : IDisposable
    {
        private readonly IUMControlAdapter umcontrolAdapter;
        private readonly LogService logService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly BalanceNotificationService notificationService;

        private Timer timer;
        private bool isOnProcessing;

        public NegativeBalanceService(
            IUMControlAdapter umcontrolAdapter,
            LogService logService,
            ApplicationConfigurationService appConfigService,
            BalanceNotificationService notificationService)
        {
            this.umcontrolAdapter = umcontrolAdapter;
            this.logService = logService;
            this.appConfigService = appConfigService;
            this.notificationService = notificationService;
            Register();
        }

        ~NegativeBalanceService()
        {
            Dispose(false);
        }

        public bool Start()
        {
            timer.Start();
            return true;
        }

        public bool Stop()
        {
            timer.Stop();
            Dispose();
            return true;
        }

        private void Register()
        {
            timer = new Timer();
            timer.Interval = appConfigService.Configurations.IntervalInSecond * 1000;
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
                notificationService.Execute(appConfigService.Configurations.IsTest);
                isOnProcessing = false;
            }
            catch (Exception ex)
            {
                isOnProcessing = false;
                logService.Log(ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }
}