using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using NLog;

namespace Auth.FWT.Infrastructure.Logging
{
    public class NLogCommandInterceptor : IDbCommandInterceptor
    {
        private static readonly Logger Logger = LogManager.GetLogger("DbExceptions");

        public void NonQueryExecuted(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void NonQueryExecuting(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ReaderExecuted(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ReaderExecuting(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ScalarExecuted(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ScalarExecuting(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        private void LogIfError<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                Logger.Error("Command {0} failed with exception {1}", command.CommandText, interceptionContext.Exception);
            }
        }

        private void LogIfNonAsync<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
        }
    }
}
