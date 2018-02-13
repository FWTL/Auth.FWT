using System;
using Auth.FWT.Core;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class AppTelegramClient
    {
        private static readonly Lazy<AppTelegramClient> LazyAppTelegramClient = new Lazy<AppTelegramClient>(() => new AppTelegramClient());

        private static readonly Lazy<TelegramClient> LazyTelegramClient = new Lazy<TelegramClient>(() =>
        {
            var instance = new TelegramClient(ConfigKeys.TelegramApiId, ConfigKeys.TelegramApiHash, new FakeSessionStore());
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

        public TelegramClient TelegramClient
        {
            get { return LazyTelegramClient.Value; }
        }
    }
}