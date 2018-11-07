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
        private int _userId;

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
                return conn.QueryFirst<byte[]>($"SELECT {Session} FROM {TelegramSession} WHERE {UserId} = @UserId", new { UserId = _userId });
            });
        }

        public async Task Save(byte[] session)
        {
            await _database.ExecuteAsync(conn =>
            {
                return conn.QueryAsync<byte[]>($@"
                IF EXISTS ( SELECT 1 FROM {TelegramSession} WHERE {UserId} = @UserId)
                BEGIN
                  INSERT INTO {TelegramSession} ({UserId},{Session})
                  VALUES (@UserId,@Session)
                END
                	ELSE
                BEGIN
                  UPDATE {TelegramSession}
                  SET {Session} = @Session
                  WHERE {UserId} = @UserId
                END);
            ", new { UserId = _userId, session });
            });
        }

        public void SetSessionTag(string sessionTag)
        {
            _userId = sessionTag.To<int>();
        }
    }
}