using System.ComponentModel.DataAnnotations;

namespace FWT.Core.Entities
{
    public class TelegramSession
    {
        [Key]
        public long UserId { get; set; }

        public byte[] Session { get; set; }
    }

    public static class TelegramSessionMap
    {
        public static string TelegramSession = nameof(TelegramSession);
        public static string UserId = nameof(UserId);
        public static string Session = nameof(Session);
    }
}