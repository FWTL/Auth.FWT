using System;
using System.Collections.Generic;
using TeleSharp.TL;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.API.Controllers.Job
{
    public class TelegramMessage
    {
        public TelegramMessage(TLMessage message)
        {
            Id = message.Id;
            CreatDateUTC = DateTimeOffset.FromUnixTimeSeconds(message.Date).UtcDateTime;
            EditDateUTC = message.EditDate.HasValue ? DateTimeOffset.FromUnixTimeSeconds(message.Date).UtcDateTime : (DateTime?)null;
            FromId = message.FromId ?? message.ViaBotId ?? -1;
            Message = message.Message;
            ParseEntities(message.Entities);
            ParseMedia(message.Media);
        }

        public TelegramMessage(TLMessageService message)
        {
            Id = message.Id;
            CreatDateUTC = DateTimeOffset.FromUnixTimeSeconds(message.Date).UtcDateTime;
            FromId = message.FromId ?? -1;
            ParseAction(message.Action);
        }

        public DateTime CreatDateUTC { get; set; }

        public DateTime? EditDateUTC { get; set; }

        public List<TelegramEntity> Entities { get; set; } = new List<TelegramEntity>();

        public int FromId { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        public TelegramMessageAction MessageAction { get; set; }

        private void ParseAction(TLAbsMessageAction action)
        {
            if (action != null)
            {
                MessageAction = MessageActionParser.Parse(action);
            }
        }

        private void ParseEntities(TLVector<TLAbsMessageEntity> entities)
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    Entities.Add(MessageEntityParser.Switch(entity));
                }
            }
        }

        private void ParseMedia(TLAbsMessageMedia media)
        {
            if (media != null)
            {
                MediaParser.Parse(media);
            }
        }
    }
}
