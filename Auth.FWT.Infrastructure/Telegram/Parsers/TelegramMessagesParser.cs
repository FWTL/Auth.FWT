using System.Collections.Generic;
using Auth.FWT.Core.Services.Telegram;
using TeleSharp.TL;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.Infrastructure.Telegram.Parsers
{
    public class TelegramMessagesParser : ITelegramMessagesParser
    {
        public TelegramMessage ParseMessage(TLAbsMessage message)
        {
            if (message is TLMessage)
            {
                var tlmessage = message as TLMessage;
                var parsedMessage = new TelegramMessage(tlmessage);
                parsedMessage.Entities = ParseEntities(tlmessage.Entities);
                parsedMessage.Media = ParseMedia(tlmessage.Media);
                return parsedMessage;
            }
            else if (message is TLMessageService)
            {
                var tlmessage = message as TLMessageService;
                var parsedMessage = new TelegramMessage(tlmessage);
                parsedMessage.MessageAction = ParseAction(tlmessage.Action);
                return parsedMessage;
            }

            return null;
        }

        private Core.Services.Telegram.TelegramMessageAction ParseAction(TLAbsMessageAction action)
        {
            if (action != null)
            {
                return MessageActionParser.Parse(action);
            }

            return null;
        }

        private List<TelegramEntity> ParseEntities(TLVector<TLAbsMessageEntity> entities)
        {
            List<TelegramEntity> parsedEntities = new List<TelegramEntity>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    parsedEntities.Add(MessageEntityParser.Switch(entity));
                }
            }

            return parsedEntities;
        }

        private ITelegramMedia ParseMedia(TLAbsMessageMedia media)
        {
            if (media != null)
            {
                return MediaParser.Parse(media);
            }

            return null;
        }
    }
}