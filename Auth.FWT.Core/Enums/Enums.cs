using System.ComponentModel;

namespace Auth.FWT.Core.Enums
{
    public static class Enum
    {
        public enum UserRole
        {
            Admin = 1,
            User = 2,
        }

        public enum DateRange
        {
            [Description("All")]
            All = 1,

            [Description("Today")]
            Today = 2,

            [Description("Yesterday")]
            Yesterday = 3,

            [Description("This Week")]
            ThisWeek = 4,

            [Description("Last Week")]
            LastWeek = 5,

            [Description("Last 7 days")]
            Last7Days = 6,

            [Description("This Month")]
            ThisMonth = 7,

            [Description("Last Month")]
            LastMonth = 8,

            [Description("Last 30 days")]
            Last30Days = 9,

            [Description("Given Month")]
            MonthYear = 10,

            [Description("Custom")]
            Custom = 11,
        }

        public enum TelegramEntity
        {
            Unknown = 0,
            Mention = 1,
            Hashtag = 2,
            BotCommand = 3,
            Url = 4,
            Email = 5,
            Bold = 6,
            Italic = 7,
            Code = 8,
            Pre = 9,
            TextUrl = 10,
            MentionName = 11,
            InputMentionName = 12,
        }

        public enum TelegramMessageAction
        {
            Empty = 0,
            ChatCreate = 1,
            ChatEditTitle = 2,
            ChatEditPhoto = 3,
            ChatDeletePhoto = 4,
            ChatAddUser = 5,
            ChatDeleteUser = 6,
            ChatJoinedByLink = 7,
            ChannelCreate = 8,
            ChatMigrateTo = 9,
            MigrateFrom = 10,
            PinMessage = 11,
            HistoryClear = 12,
            GameScore = 13,
            PaymentSentMe = 14,
            PaymentSent = 15,
            PhoneCall = 16,
            ScreenshotTaken = 17,
            ActionCustomAction = 18,
        }
    }
}