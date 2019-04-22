using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using Fanex.ServiceMonitoring.Models;

namespace Fanex.ServiceMonitoring.Services
{
    public interface RestartService
    {
        void Execute(List<ServiceModel> services);
    }

    public class RestartServiceImp : RestartService
    {        
        private const int Milisecond = 1000;
        private const int SleepingTime = 15;

        private readonly Log log;

        public RestartServiceImp(Log log)
        {
            this.log = log;
        }

        public void Execute(List<ServiceModel> services)
        {
            foreach (var service in services)
            {
                try
                {
                    Kill(service.ServiceName);
                    Restart(service);
                }
                catch (Exception exception)
                {
                    service.Description = exception.Message;
                    log.Log(exception);
                }
            }
        }

        private static void Kill(string serviceName)
        {
            string query = $"SELECT ProcessId FROM Win32_Service WHERE Name='{serviceName}'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj in searcher.Get().OfType<ManagementObject>())
            {
                uint processId = (uint)obj["ProcessId"];
                Process process = null;
                process = Process.GetProcessById((int)processId);
                if (process != null && processId > 0)
                {
                    process.Kill();
                }
            }
        }

        private static void Restart(ServiceModel service)
        {
            var result = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == service.ServiceName);
            if (result != null)
            {
                result.Start();

                System.Threading.Thread.Sleep(SleepingTime * Milisecond);
                result = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == service.ServiceName);
                if (result.Status != ServiceControllerStatus.Running)
                {
                    service.Description = $"The service {service.ServiceName} has been restarted UNSUCCESSFULLY, status always Starting or {result.Status.ToString()} after {SleepingTime}s";
                    service.IsRestartSuccessfully = false;
                }
                else
                {
                    service.Description = $"Restarted successfully";
                    service.IsRestartSuccessfully = true;
                }
            }
            else
            {
                service.Description = $"The Service {service.ServiceName} doesn't exist on the server {service.IPServer}";
            }
        }
    }
}