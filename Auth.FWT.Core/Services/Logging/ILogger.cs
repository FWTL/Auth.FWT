using System;

namespace Auth.FWT.Core.Services.Logging
{
    public interface ILogger
    {
        void Log(Exception ex);

        void Log(string message);

        void LogTrace(string message);
    }
}
