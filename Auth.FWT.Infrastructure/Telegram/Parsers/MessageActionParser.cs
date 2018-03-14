using System;
using System.Collections.Generic;
using Auth.FWT.Core.Services.Telegram;
using TeleSharp.TL;

namespace Auth.FWT.Infrastructure.Telegram.Parsers
{
    public static class MessageActionParser
    {
        private static readonly Dictionary<string, Func<TLAbsMessageAction, TelegramMessageAction>> Switch = new Dictionary<string, Func<TLAbsMessageAction, TelegramMessageAction>>()
        {
            { typeof(TLMessageActionEmpty).FullName, x => { return Parse(x as TLMessageActionEmpty); } },
            { typeof(TLMessageActionChatCreate).FullName, x => { return Parse(x as TLMessageActionChatCreate); } },
            { typeof(TLMessageActionChatEditTitle).FullName, x => { return Parse(x as TLMessageActionChatEditTitle); } },
            { typeof(TLMessageActionChatEditPhoto).FullName, x => { return Parse(x as TLMessageActionChatEditPhoto); } },
            { typeof(TLMessageActionChatDeletePhoto).FullName, x => { return Parse(x as TLMessageActionChatDeletePhoto); } },
            { typeof(TLMessageActionChatAddUser).FullName, x => { return Parse(x as TLMessageActionChatAddUser); } },
            { typeof(TLMessageActionChatDeleteUser).FullName, x => { return Parse(x as TLMessageActionChatDeleteUser); } },
            { typeof(TLMessageActionChatJoinedByLink).FullName, x => { return Parse(x as TLMessageActionChatJoinedByLink); } },
            { typeof(TLMessageActionChannelCreate).FullName, x => { return Parse(x as TLMessageActionChannelCreate); } },
            { typeof(TLMessageActionChatMigrateTo).FullName, x => { return Parse(x as TLMessageActionChatMigrateTo); } },
            { typeof(TLMessageActionChannelMigrateFrom).FullName, x => { return Parse(x as TLMessageActionChannelMigrateFrom); } },
            { typeof(TLMessageActionPinMessage).FullName, x => { return Parse(x as TLMessageActionPinMessage); } },
            { typeof(TLMessageActionHistoryClear).FullName, x => { return Parse(x as TLMessageActionHistoryClear); } },
            { typeof(TLMessageActionGameScore).FullName, x => { return Parse(x as TLMessageActionGameScore); } },
            { typeof(TLMessageActionPaymentSentMe).FullName, x => { return Parse(x as TLMessageActionPaymentSentMe); } },
            { typeof(TLMessageActionPaymentSent).FullName, x => { return Parse(x as TLMessageActionPaymentSent); } },
            { typeof(TLMessageActionPhoneCall).FullName, x => { return Parse(x as TLMessageActionPhoneCall); } },
        };

        public static TelegramMessageAction Parse(TLAbsMessageAction messageAction)
        {
            return Switch[messageAction.GetType().FullName](messageAction);
        }

        private static TelegramMessageAction Parse(TLMessageActionChannelCreate tLMessageActionChannelCreate)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChannelCreate,
                ActionMessage = $"Created {tLMessageActionChannelCreate.Title}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChannelMigrateFrom tLMessageActionChannelMigrateFrom)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.MigrateFrom,
                ActionMessage = $"Migrated from {tLMessageActionChannelMigrateFrom.Title} id:{tLMessageActionChannelMigrateFrom.ChatId}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatAddUser tLMessageActionChatAddUser)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatAddUser,
                ActionMessage = $"Users joined {string.Join(", ", tLMessageActionChatAddUser.Users)}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatCreate tLMessageActionChatCreate)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatCreate,
                ActionMessage = $"New Chat created : {tLMessageActionChatCreate.Title} Users joined {string.Join(", ", tLMessageActionChatCreate.Users)}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatDeletePhoto tLMessageActionChatDeletePhoto)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatDeletePhoto,
                ActionMessage = $"Photo deleted"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatDeleteUser tLMessageActionChatDeleteUser)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatDeleteUser,
                ActionMessage = $"User Deleted {tLMessageActionChatDeleteUser.UserId}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatEditPhoto tLMessageActionChatEditPhoto)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatEditPhoto,
                ActionMessage = $"Photo edited"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatEditTitle tLMessageActionChatEditTitle)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatEditTitle,
                ActionMessage = $"New title : {tLMessageActionChatEditTitle.Title}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatJoinedByLink tLMessageActionChatJoinedByLink)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatJoinedByLink,
                ActionMessage = $"Invited by {tLMessageActionChatJoinedByLink.InviterId}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionChatMigrateTo tLMessageActionChatMigrateTo)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.ChatMigrateTo,
                ActionMessage = $"Migrated to id:{tLMessageActionChatMigrateTo.ChannelId}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionEmpty tLMessageActionEmpty)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.Empty,
                ActionMessage = string.Empty
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionGameScore tLMessageActionGameScore)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.GameScore,
                ActionMessage = $"Score {tLMessageActionGameScore.Score} in game {tLMessageActionGameScore.Score}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionHistoryClear tLMessageActionHistoryClear)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.HistoryClear,
                ActionMessage = $"History Cleared"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionPaymentSent tLMessageActionPaymentSent)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.PaymentSent,
                ActionMessage = $"Sent {tLMessageActionPaymentSent.TotalAmount} {tLMessageActionPaymentSent.Currency}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionPaymentSentMe tLMessageActionPaymentSentMe)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.PaymentSentMe,
                ActionMessage = $"Sent {tLMessageActionPaymentSentMe.TotalAmount} {tLMessageActionPaymentSentMe.Currency}"
            };
        }

        private static TelegramMessageAction Parse(TLMessageActionPhoneCall tLMessageActionPhoneCall)
        {
            var result = new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.PhoneCall
            };

            if (tLMessageActionPhoneCall.Reason == null)
            {
                result.ActionMessage = $"Phone call took {tLMessageActionPhoneCall.Duration} s.";
                return result;
            }

            if (tLMessageActionPhoneCall.Reason is TLPhoneCallDiscardReasonMissed)
            {
                result.ActionMessage = $"Phone call missed";
                return result;
            }

            if (tLMessageActionPhoneCall.Reason is TLPhoneCallDiscardReasonDisconnect)
            {
                result.ActionMessage = $"Phone call disconnected";
                return result;
            }

            if (tLMessageActionPhoneCall.Reason is TLPhoneCallDiscardReasonHangup)
            {
                result.ActionMessage = $"Phone call hangup";
                return result;
            }

            if (tLMessageActionPhoneCall.Reason is TLPhoneCallDiscardReasonBusy)
            {
                result.ActionMessage = $"Phone call busy";
                return result;
            }

            return result;
        }

        private static TelegramMessageAction Parse(TLMessageActionPinMessage tLMessageActionPinMessage)
        {
            return new TelegramMessageAction()
            {
                Type = Core.Enums.Enum.TelegramMessageAction.PinMessage,
                ActionMessage = $"Message pinned"
            };
        }
    }
}
