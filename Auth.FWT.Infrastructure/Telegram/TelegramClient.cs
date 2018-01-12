using System;
using Auth.FWT.Core;
using Auth.FWT.Core.Services.Telegram;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Logging
{
    public class AppTelegramClient : IAppTelegramClient
    {
        private static readonly Lazy<AppTelegramClient> LazyAppTelegramClient = new Lazy<AppTelegramClient>(() => new AppTelegramClient());

        private static readonly Lazy<TelegramClient> LazyTelegramClient = new Lazy<TelegramClient>(() => new TelegramClient(ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash));

        private AppTelegramClient()
        {
        }

        public static AppTelegramClient Instance
        {
            get
            {
                return LazyAppTelegramClient.Value;
            }
        }

        public TelegramClient Client
        {
            get
            {
                if (!LazyTelegramClient.Value.IsConnected)
                {
                    LazyTelegramClient.Value.ConnectAsync();
                }

                return LazyTelegramClient.Value;
            }
        }
    }
}