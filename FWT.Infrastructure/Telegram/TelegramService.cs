using FWT.Core.Services.Dapper;
using FWT.Core.Services.Telegram;
using OpenTl.ClientApi;
using System.Threading.Tasks;

namespace FWT.Infrastructure.Telegram
{
    public class TelegramService : ITelegramService
    {
        private readonly IDatabaseConnector _database;
        private readonly TelegramSettings _settings;

        public TelegramService(IDatabaseConnector database,  TelegramSettings settings)
        {
            _database = database;
            _settings = settings;
        }

        private IFactorySettings BuildSettings(int userId)
        {
            return new FactorySettings()
            {
                AppHash = _settings.AppHash,
                AppId = _settings.AppId,
                ServerAddress = _settings.ServerAddress,
                ServerPublicKey = _settings.ServerPublicKey,
                ServerPort = _settings.ServerPort,
                SessionTag = userId.ToString(),
                Properties = new ApplicationProperties()
                {
                    AppVersion = "1.0.0",
                    DeviceModel = "Server",
                    SystemLangCode = "en",
                    LangCode = "en",
                    LangPack = "en",
                    SystemVersion = "Windows",
                },
                SessionStore = new DatabaseSessionStore(_database)
            };
        }

        public async Task<IClientApi> Build(int userId)
        {
            return await ClientFactory.BuildClientAsync(BuildSettings(userId));
        }
    }
}