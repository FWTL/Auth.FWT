using FWT.Core.Helpers;
using FWT.Core.Services.Dapper;
using FWT.Core.Services.Telegram;
using OpenTl.ClientApi;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Telegram
{
    public class TelegramService : ITelegramService
    {
        private readonly IDatabaseConnector _database;
        private readonly TelegramSettings _settings;
        private readonly IDatabase _cache;

        public TelegramService(IDatabaseConnector database, TelegramSettings settings, IDatabase cache)
        {
            _database = database;
            _settings = settings;
            _cache = cache;
        }

        private IFactorySettings BuildSettings(string hash)
        {
            return new FactorySettings()
            {
                AppHash = _settings.AppHash,
                AppId = _settings.AppId,
                ServerAddress = _settings.ServerAddress,
                ServerPublicKey = _settings.ServerPublicKey,
                ServerPort = _settings.ServerPort,
                SessionTag = hash,
                Properties = new ApplicationProperties()
                {
                    AppVersion = "1.0.0",
                    DeviceModel = "Server",
                    SystemLangCode = "en",
                    LangCode = "en",
                    LangPack = "tdesktop",
                    SystemVersion = "Windows",
                },
                SessionStore = new DatabaseSessionStore(_database)
            };
        }

        public async Task<IClientApi> Build(string hash)
        {
            RedisValue redisResult = _cache.StringGet(hash);
            if (redisResult.HasValue)
            {
                ObjectHelper.FromByteArray<IClientApi>(redisResult);
            }

            IClientApi client = await ClientFactory.BuildClientAsync(BuildSettings(hash));
            _cache.StringSet(hash, ObjectHelper.ToByteArray(client));

            return client;
        }
    }
}