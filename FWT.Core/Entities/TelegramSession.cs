using System.ComponentModel.DataAnnotations;

namespace FWT.Core.Entities
{
    public class TelegramSession
    {
        [Key]
        public long UserId { get; set; }

        public byte[] Session { get; set; }
    }

   
}