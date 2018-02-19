using System;
using Auth.FWT.Core;
using TLSharp.Core;
using TLSharp.Custom;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class AppTelegramClient
    {
        private static readonly Lazy<AppTelegramClient> LazyAppTelegramClient = new Lazy<AppTelegramClient>(() => new AppTelegramClient());

        private static readonly Lazy<NewTelegramClient> LazyTelegramClient = new Lazy<NewTelegramClient>(() =>
        {
            var instance = new NewTelegramClient(ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash);
            return instance;
        });

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

        public ITelegramClient TelegramClient
        {
            get { return LazyTelegramClient.Value; }
        }
    }
}