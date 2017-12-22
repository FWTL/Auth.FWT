using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Auth.FWT.Core.Services.Dapper;

namespace Auth.FWT.Data.Dapper
{
    public class DapperConnector : IDapperConnector
    {
        private readonly string _connectionString;

        public DapperConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<T> Execute<T>(Func<IDbConnection, Task<T>> data)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await data(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        public async Task Execute(Func<IDbConnection, Task> data)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await data(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(string.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }
    }
}
