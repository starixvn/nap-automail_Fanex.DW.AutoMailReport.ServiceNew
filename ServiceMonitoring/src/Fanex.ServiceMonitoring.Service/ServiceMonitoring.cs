using System;
using System.Net;
using System.Timers;
using Fanex.ServiceMonitoring.Criterias;
using Fanex.ServiceMonitoring.Services;
using ONEbook.UM.Services;

namespace Fanex.ServiceMonitoring.Service
{
    public class ServiceMonitoring
    {
        private const int Milliseconds = 1000;

        private readonly IUMControlAdapter umcontrolAdapter;
        private readonly DataService dataService;
        private readonly RestartService restartService;
        private readonly MailService mailService;        
        private readonly Log log;

        private Timer timer;
        private bool isOnProcessing;

        public ServiceMonitoring(
            IUMControlAdapter umcontrolAdapter,
            DataService dataService,
            RestartService restartService,
            MailService mailService,
            Log log)
        {
            this.umcontrolAdapter = umcontrolAdapter;
            this.dataService = dataService;
            this.restartService = restartService;
            this.mailService = mailService;
            this.log = log;

            Register();
        }

        public void Start()
        {
            timer.Start();            
        }
     
        private void Register()
        {
            timer = new Timer {Interval = AppConfigService.AppConfig.IntervalInSecondServiceMonitor * Milliseconds};
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

                CheckIn();

                var services = dataService.QueryService();
                restartService.Execute(services);
                dataService.WriteLogServiceRestartTime(services);
                mailService.SendMail(services);

                isOnProcessing = false;
            }
            catch (Exception exception)
            {
                isOnProcessing = false;
                log.Log(exception);
            }
        }

        private void CheckIn()
        {
            var criteria = new ServiceMonitoringCheckInCriteria
            {
                Interval = AppConfigService.AppConfig.IntervalInSecondServiceMonitor,
                IPServer = GetLocalIp(),
                OrderBy = 255,
                ServiceName = "Fanex.ServiceMonitoring.Service-MAIN"
            };

            dataService.CheckIn(criteria);
        }

        private static string GetLocalIp()
        {
            var ip = string.Empty;
            var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var addr in localIPs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = addr.ToString();
                }
            }

            return ip;
        }
    }
}