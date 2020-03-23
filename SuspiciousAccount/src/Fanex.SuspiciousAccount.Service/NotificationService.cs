using Fanex.SuspiciousAccount.Business.Criterias;
using Fanex.SuspiciousAccount.Business.Services;
using ONEbook.UM.Services;
using System;
using System.Linq;
using System.Net;
using System.Timers;

namespace Topshelf.SimpleInjector.QuickStart.Content
{
    public class NotificationService
    {
        private readonly IUMControlAdapter umcontrolAdapter;
        private readonly Log logService;
        private readonly DataService dataService;
        private readonly MessageService messageService;
        private readonly EvidenceAccountService evidenceAccountService;

        private Timer timer;
        private bool isOnProcessing;
        private const int RowPerBatch = 100;

        public NotificationService(
            IUMControlAdapter umcontrolAdapter,
            Log logService,
            DataService dataService,
            MessageService messageService,
            EvidenceAccountService evidenceAccountService)
        {
            this.umcontrolAdapter = umcontrolAdapter;
            this.logService = logService;
            this.dataService = dataService;
            this.messageService = messageService;
            this.evidenceAccountService = evidenceAccountService;

            Register();
        }

        ~NotificationService()
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
            timer.Interval = AppConfigService.Config.IntervalInSecond * 1000;
            timer.Elapsed += (sender, e) =>
            {
                CheckIn();
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
                Notify1();
                Notify2();
                isOnProcessing = false;
            }
            catch (Exception ex)
            {
                isOnProcessing = false;
                logService.Log(ex);
            }
        }

        private void Notify1()
        {
            var allAccounts = dataService.QueryAccount().Where(s => s.NotifyStatus1 == 1).ToList();
            var rowPerBatch = RowPerBatch;
            var numberOfBatch = allAccounts.Count / rowPerBatch + ((allAccounts.Count % rowPerBatch) > 0 ? 1 : 0);

            for (var batchIndex = 0; batchIndex < numberOfBatch; batchIndex++)
            {
                var rowsOfBatch = allAccounts.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var evidenceAccounts = evidenceAccountService.GetEvidenceAccount(rowsOfBatch);
                if (evidenceAccounts.Count == 0)
                {
                    continue;
                }

                var result = messageService.SendNotifyStatus1(evidenceAccounts);
                if (result)
                {
                    dataService.UpdateAccount(evidenceAccounts);
                }
            }
        }

        private void Notify2()
        {
            var allAccounts = dataService.QueryAccount().Where(s => s.NotifyStatus2 == 1).ToList();
            var rowPerBatch = RowPerBatch;
            var numberOfBatch = allAccounts.Count / rowPerBatch + ((allAccounts.Count % rowPerBatch) > 0 ? 1 : 0);

            for (var batchIndex = 0; batchIndex < numberOfBatch; batchIndex++)
            {
                var rowsOfBatch = allAccounts.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var evidenceAccounts = evidenceAccountService.GetEvidenceAccount(rowsOfBatch);
                if (evidenceAccounts.Count == 0)
                {
                    continue;
                }

                var result = messageService.SendNotifyStatus2(evidenceAccounts);
                if (result)
                {
                    dataService.UpdateAccount(evidenceAccounts);                    
                }
            }
        }

        private void CheckIn()
        {
            var serviceName = AppConfigService.Config.ServiceName;
            var criteria = new ServiceMonitoringCheckInCriteria
            {
                Interval = AppConfigService.Config.IntervalInSecond,
                IPServer = GetLocalIP(),
                OrderBy = 3,
                ServiceName = serviceName
            };

            dataService.CheckIn(criteria);
        }

        private static string GetLocalIP()
        {
            var ip = string.Empty;
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = addr.ToString();
                }
            }

            return ip;
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