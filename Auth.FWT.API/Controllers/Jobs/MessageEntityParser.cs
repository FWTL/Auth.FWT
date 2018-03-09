using System.Collections.Generic;
using TeleSharp.TL;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.API.Controllers.Jobs
{
    public static class MessageEntityParser
    {
        private static readonly Dictionary<string, TelegramEntity> SwitchDictionary = new Dictionary<string, TelegramEntity>()
        {
            { typeof(TLMessageEntityUnknown).FullName, TelegramEntity.Unknown },
            { typeof(TLMessageEntityMention).FullName, TelegramEntity.Mention },
            { typeof(TLMessageEntityHashtag).FullName, TelegramEntity.Hashtag },
            { typeof(TLMessageEntityBotCommand).FullName, TelegramEntity.BotCommand },
            { typeof(TLMessageEntityUrl).FullName, TelegramEntity.Url },
            { typeof(TLMessageEntityEmail).FullName, TelegramEntity.Email },
            { typeof(TLMessageEntityBold).FullName, TelegramEntity.Bold },
            { typeof(TLMessageEntityItalic).FullName, TelegramEntity.Italic },
            { typeof(TLMessageEntityCode).FullName, TelegramEntity.Code },
            { typeof(TLMessageEntityPre).FullName, TelegramEntity.Pre },
            { typeof(TLMessageEntityTextUrl).FullName, TelegramEntity.TextUrl },
            { typeof(TLMessageEntityMentionName).FullName, TelegramEntity.MentionName },
            { typeof(TLInputMessageEntityMentionName).FullName, TelegramEntity.InputMentionName },
        };

        public static TelegramEntity Switch(TLAbsMessageEntity message)
        {
            return SwitchDictionary[message.GetType().FullName];
        }
    }
}