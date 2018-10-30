using System;
using System.Data;
using System.Threading.Tasks;

namespace FWT.Core.Services.Dapper
{
    public interface IDatabaseConnector
    {
        void Execute(Action<IDbConnection> data);

        T Execute<T>(Func<IDbConnection, T> data);

        Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> data);

        Task ExecuteAsync(Func<IDbConnection, Task> data);
    }
}
