using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Fanex.Logging;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface LogService
    {
        void Log(Exception exception);
    }

    public class LogServiceImp : LogService
    {
        public void Log(Exception exception)
        {
            var logInfo = BuildLogMessage(exception);

            Logger.Log.Error(string.Join(Environment.NewLine, logInfo));
        }

        public virtual List<string> BuildLogMessage(Exception exception)
        {
            var sqlException = exception as SqlException;
            var stackTrace = new StackTrace(exception).GetFrames();
            var methodName = stackTrace == null ? string.Empty : stackTrace.Select(f => f.GetMethod()).FirstOrDefault().Name;
            var logInfo = new List<string>
            {
                $"Message: {exception.Message}",
                $"SOURCE: {exception.Source}",
                $"MethodName: {methodName}",
                $"TARGETSITE: {exception.TargetSite}",
                $"STACKTRACE: {exception.StackTrace}",
            };

            if (sqlException != null)
            {
                logInfo.Add($"Procedure: {sqlException.Procedure}");
                logInfo.Add($"Line Number: {sqlException.LineNumber}");
                logInfo.Add($"Server: {sqlException.Server}");
            }

            return logInfo;
        }
    }
}