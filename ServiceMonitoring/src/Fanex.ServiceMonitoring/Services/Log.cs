using System;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using ONEbook.UM.Services;

namespace Fanex.ServiceMonitoring.Services
{
    public interface Log
    {
        void Log(Exception exception);

        void Log(string message);
    }

    public class LogImp : Log
    {
        private readonly IUMControlAdapter umcontrolAdapter;

        public LogImp(IUMControlAdapter umcontrolAdapter)
        {
            this.umcontrolAdapter = umcontrolAdapter;
        }

        public void Log(string message)
        {
            if (umcontrolAdapter.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
            {
                return;
            }

            LogService.Info(message);
        }

        public void Log(Exception exception)
        {
            if (umcontrolAdapter.GetUnderMaintenanceTime().IsUnderMaintenanceTime)
            {
                return;
            }

            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                        .ToString();

            LogService.Error(logMessage);
        }
    }
}