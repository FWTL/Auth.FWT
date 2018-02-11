using Auth.FWT.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSharp.Core;

namespace Auth.FWT.Infrastructure.Telegram
{
    public class AppTelegramClient
    {
        private static readonly Lazy<AppTelegramClient> LazyAppTelegramClient = new Lazy<AppTelegramClient>(() => new AppTelegramClient());

        private static readonly Lazy<TelegramClient> LazyTelegramClient = new Lazy<TelegramClient>(() => new TelegramClient(ConfigKeys.TelegramApiId,ConfigKeys.TelegramApiHash, new FakeSessionStore()));

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

        public void Debug(Exception exception, string message = null, params object[] args)
        {
            LazyAppTelegramClient.Value;
        }
    }
}
