using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using System;
using System.IO;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface Log
    {
        void Log(Exception exception);

        void Log(string message);
    }

    public class LogImp : Log
    {
        public void Log(string message)
        {
            Directory.CreateDirectory(AppConfigService.AppConfig.FilePath);
            File.WriteAllText($@"{AppConfigService.AppConfig.FilePath}\Log.txt", message);
            LogService.Info(message);
        }

        public void Log(Exception exception)
        {
            Directory.CreateDirectory(AppConfigService.AppConfig.FilePath);
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                        .ToString();
            File.WriteAllText($@"{AppConfigService.AppConfig.FilePath}\ERROR.txt", logMessage);
            LogService.Error(logMessage);
        }
    }
}