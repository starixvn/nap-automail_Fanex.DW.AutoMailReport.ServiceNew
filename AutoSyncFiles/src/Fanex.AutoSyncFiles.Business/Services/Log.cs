using Fanex.Logging;
using System;

namespace Fanex.AutoSyncFiles.Business.Services
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
            LogService.Info(message);
        }

        public void Log(Exception exception)
        {
            LogService.Error(exception.Message);
        }
    }
}