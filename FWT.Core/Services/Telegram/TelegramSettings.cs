using System;
using System.Collections.Generic;
using System.Text;

namespace FWT.Core.Services.Telegram
{
    public class TelegramSettings
    {
        public string AppHash { get; set; }
        public int AppId { get; set; }
        public string ServerAddress { get; set; }
        public string ServerPublicKey { get; set; }
        public int ServerPort { get; set; }
    }
}
