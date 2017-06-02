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
            var sqlException = exception as SqlException;
            var stackTrace = new StackTrace(exception).GetFrames();
            var methodbase = stackTrace.Select(f => f.GetMethod()).FirstOrDefault();
            var methodName = methodbase == null ? string.Empty : methodbase.Name;
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

            Logger.Log.Error(string.Join(Environment.NewLine, logInfo));
        }
    }
}