using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Fanex.ServiceMonitoring.Models;
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

        public ServiceMonitoring(IUMControlAdapter umcontrolAdapter, DataService dataService, RestartService restartService, MailService mailService, Log log)
        {
            this.umcontrolAdapter = umcontrolAdapter;
            this.dataService = dataService;
            this.restartService = restartService;
            this.mailService = mailService;
            this.log = log;

            Register();
        }

        public bool Start()
        {
            timer.Start();
            return true;
        }

        public bool Stop()
        {
            timer.Stop();
            return true;
        }

        private void Register()
        {
            timer = new Timer();
            timer.Interval = AppConfigService.AppConfig.IntervalInSecond * Milliseconds;
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
            List<ServiceModel> services;

            try
            {
                if (umcontrolAdapter.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
                {
                    return;
                }

                isOnProcessing = true;

                services = dataService.QueryService();
                restartService.Execute(services);
                dataService.WriteLogServiceRestartTime(services);
                mailService.SendMail(services);

                isOnProcessing = false;
            }
            catch (Exception exception)
            {                
                isOnProcessing = false;
                
                File.WriteAllText(@"E:\1.Service\2.Nap-Reports-Auto-Mail-Services\ServiceMonitoring.txt", exception.Message);
               // File.WriteAllText(@"D:\Web\10.Nap-AutoMail\ServiceMonitoring.txt", exception.Message);

                log.Log(exception);
            }
        }
    }
}