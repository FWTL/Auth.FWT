using System;
using System.Collections.Generic;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.Core.Services.Telegram
{
    public class TelegramMessage
    {
        public TelegramMessage()
        {
        }

        public DateTime CreatDateUTC { get; set; }

        public DateTime? EditDateUTC { get; set; }

        public List<TelegramEntity> Entities { get; set; } = new List<TelegramEntity>();

        public int FromId { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        public TelegramMessageAction MessageAction { get; set; }
    }
}