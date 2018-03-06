using System;

namespace Auth.FWT.Events
{
    public class TelegramFetchingMessagesJobStarted
    {
        public Guid JobId { get; set; }
        public int InvokedBy { get; set; }
        public ChannalType ChannalType { get; set; }
        public int ChannalId { get; set; }
    }

    public enum ChannalType
    {
        User = 1,
        Chat = 2,
        Channal = 3,
    }
}