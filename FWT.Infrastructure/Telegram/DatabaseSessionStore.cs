using Dapper;
using FWT.Core.Extensions;
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
                return conn.QueryFirstOrDefault<byte[]>($"SELECT {Session} FROM {TelegramSession} WHERE {HashId} = @Hash", new { HashId = _hashId });
            });
        }

        public async Task Save(byte[] session)
        {
            await _database.ExecuteAsync(conn =>
            {
                return conn.ExecuteAsync($@"
                IF EXISTS ( SELECT 1 FROM {TelegramSession} WHERE {HashId} = @HashId)
                BEGIN
                  INSERT INTO {TelegramSession} ({HashId},{Session})
                  VALUES (@UserId,@Session)
                END
                	ELSE
                BEGIN
                  UPDATE {TelegramSession}
                  SET {Session} = @Session
                  WHERE {HashId} = @HashId
                END);
            ", new { HashId = _hashId, session });
            });
        }

        public void SetSessionTag(string sessionTag)
        {
            _hashId = sessionTag;
        }
    }
}