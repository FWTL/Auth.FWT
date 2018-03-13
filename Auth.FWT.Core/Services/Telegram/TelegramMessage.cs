using System;
using System.Collections.Generic;
using Auth.FWT.Infrastructure.Json;
using Newtonsoft.Json;
using TeleSharp.TL;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.Core.Services.Telegram
{
    public class TelegramMessage
    {
        public TelegramMessage()
        {
        }

        public TelegramMessage(TLMessageService tlmessage)
        {
            Id = tlmessage.Id;
            CreatDateUTC = DateTimeOffset.FromUnixTimeSeconds(tlmessage.Date).UtcDateTime;
            FromId = tlmessage.FromId ?? -1;
        }

        public TelegramMessage(TLMessage tlmessage)
        {
            Id = tlmessage.Id;
            CreatDateUTC = DateTimeOffset.FromUnixTimeSeconds(tlmessage.Date).UtcDateTime;
            EditDateUTC = tlmessage.EditDate.HasValue ? DateTimeOffset.FromUnixTimeSeconds(tlmessage.Date).UtcDateTime : (DateTime?)null;
            FromId = tlmessage.FromId ?? tlmessage.ViaBotId ?? -1;
            Message = tlmessage.Message;
        }

        public DateTime CreatDateUTC { get; set; }

        public DateTime? EditDateUTC { get; set; }

        public List<TelegramEntity> Entities { get; set; } = new List<TelegramEntity>();

        public int FromId { get; set; }

        public int Id { get; set; }

        [JsonConverter(typeof(TelegramMediaConverter))]
        public ITelegramMedia Media { get; set; }

        public string Message { get; set; }

        public TelegramMessageAction MessageAction { get; set; }
    }
}