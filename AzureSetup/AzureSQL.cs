using Dapper;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Sql.Fluent;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AzureSetup
{
    public class AzureSql
    {
        private IAzure _azure;
        private Options _options;

        public AzureSql(IAzure azure, Options options)
        {
            _azure = azure;
            _options = options;
        }

        public async Task<ISqlServer> CreateOrGetServerAsync(string name, IResourceGroup group, string passwordKey)
        {
            Console.WriteLine($"AzureSql.CreateOrGetServerAsync {name}");

            var sqlServer = _azure.SqlServers.GetByResourceGroup(group.Name, name);
            if (sqlServer != null)
            {
                _options.AddSettings("SQL_PASSWORD", Util.CreateRandomPassword(20));
                return sqlServer;
            }

            var login = "sqlAdmin";

            return await _azure.SqlServers.Define(name)
            .WithRegion(Region.USEast)
            .WithExistingResourceGroup(group)
            .WithAdministratorLogin(login)
            .WithAdministratorPassword(_options.AddSettings(passwordKey, Util.CreateRandomPassword(20)))
            .WithNewFirewallRule("0.0.0.0", "255.255.255.255")
            .CreateAsync();
        }

        public async Task<ISqlDatabase> CreateOrGetDatabase(string name, ISqlServer sqlServer, SqlDatabaseBasicStorage capacity)
        {
            Console.WriteLine($"AzureSql.CreateOrGetDatabase {name}");

            var database = sqlServer.Databases.Get(name);
            if (database != null)
            {
                return database;
            }

            return await sqlServer.Databases.Define(name).WithBasicEdition(capacity).CreateAsync();
        }

        public async Task CreateDbUser(ISqlServer server, ISqlDatabase database, string password)
        {
            Console.WriteLine($"AzureSql.CreateDbUser {database.Name}");

            using (var connection = new SqlConnection($"Server=tcp:{server.Name}.database.windows.net,1433;Initial Catalog=master;Persist Security Info=False;User ID=sqlAdmin;Password={_options.GetKeyValue("SQL_PASSWORD")};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                connection.Open();
                await connection.ExecuteAsync($@"
                IF NOT EXISTS (SELECT name FROM sys.sql_logins WHERE name = '{database.Name}_login')
                BEGIN
                    CREATE LOGIN {database.Name}_login WITH password='{password}';
                END");
            }

            using (var connection = new SqlConnection($"Server=tcp:{server.Name}.database.windows.net,1433;Initial Catalog={database.Name};Persist Security Info=False;User ID=sqlAdmin;Password={_options.GetKeyValue("SQL_PASSWORD")};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                connection.Open();
                await connection.ExecuteAsync($@"
                IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = '{database.Name}_user')
                BEGIN
                    CREATE USER {database.Name}_user FROM LOGIN {database.Name}_login;
                    exec sp_addrolemember 'db_datawriter', '{database.Name}_user';
                    exec sp_addrolemember 'db_datareader', '{database.Name}_user';
                END");
            }
        }
    }
}