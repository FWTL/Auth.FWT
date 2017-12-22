using System;
using Auth.FWT.Core.Services.Logging;

namespace Auth.FWT.Infrastructure.Logging
{
    public class NLogLogger : ILogger
    {
        private static readonly Lazy<NLogLogger> LazyLogger = new Lazy<NLogLogger>(() => new NLogLogger());

        private static readonly Lazy<NLog.Logger> LazyNLogger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetLogger("AppLogger"));

        private NLogLogger()
        {
        }

        public static ILogger Instance
        {
            get
            {
                return LazyLogger.Value;
            }
        }

        public void Log(Exception ex)
        {
            LazyNLogger.Value.Error(ex);
        }

        public void Log(string message)
        {
            LazyNLogger.Value.Info(message);
        }

        public void LogTrace(string message)
        {
            LazyNLogger.Value.Trace(message);
        }
    }
}
