using FWT.Core.Services.Dapper;
using FWT.Core.Services.Telegram;
using Microsoft.Extensions.Caching.Memory;
using OpenTl.ClientApi;
using System;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Telegram
{
    public class TelegramService : ITelegramService
    {
        private readonly IDatabaseConnector _database;
        private readonly TelegramSettings _settings;
        private readonly MemoryCache _memoryCache;
        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(60);

        public TelegramService(IDatabaseConnector database, TelegramSettings settings)
        {
            _database = database;
            _settings = settings;
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
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
            if (_memoryCache.TryGetValue(hash, out IClientApi clientApi))
            {
                return clientApi;
            }

            var client = await ClientFactory.BuildClientAsync(BuildSettings(hash));
            _memoryCache.Set(hash, clientApi, new MemoryCacheEntryOptions().SetSlidingExpiration(SlidingExpiration));

            return client;
        }
    }
}