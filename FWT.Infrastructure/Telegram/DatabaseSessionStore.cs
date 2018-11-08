using Dapper;
using FWT.Core.Services.Dapper;
using OpenTl.ClientApi;
using System.Threading.Tasks;
using static FWT.Core.Entities.Maps.TelegramSessionMap;

namespace FWT.Infrastructure.Telegram
{
    public class DatabaseSessionStore : ISessionStore
    {
        private readonly IDatabaseConnector _database;
        private string _hashId;

        public DatabaseSessionStore(IDatabaseConnector database)
        {
            _database = database;
        }

        public void Dispose()
        {
        }

        public byte[] Load()
        {
            return _database.Execute(conn =>
            {
                return conn.QueryFirstOrDefault<byte[]>($"SELECT {Session} FROM {TelegramSession} WHERE {HashId} = @HashId", new { HashId = _hashId });
            });
        }

        public async Task Save(byte[] session)
        {
            await _database.ExecuteAsync(conn =>
            {
                return conn.ExecuteAsync($@"
                IF NOT EXISTS ( SELECT 1 FROM {TelegramSession} WHERE {HashId} = @{HashId})
                BEGIN
                  INSERT INTO {TelegramSession} ({HashId},{Session})
                  VALUES (@{HashId},@{Session})
                END
                	ELSE
                BEGIN
                  UPDATE {TelegramSession}
                  SET {Session} = @{Session}
                  WHERE {HashId} = @{HashId}
                END;
            ", new { HashId = _hashId, Session = session });
            });
        }

        public void SetSessionTag(string sessionTag)
        {
            _hashId = sessionTag;
        }
    }
}