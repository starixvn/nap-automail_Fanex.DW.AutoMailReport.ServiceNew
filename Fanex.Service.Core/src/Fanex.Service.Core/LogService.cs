using System;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;

namespace Fanex.Service.Core
{
    public interface LogService
    {
        void Log(Exception exception);
    }

    public class LogServiceImp : LogService
    {
        public void Log(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();

            Logging.LogService.Error(logMessage);
        }
    }
}