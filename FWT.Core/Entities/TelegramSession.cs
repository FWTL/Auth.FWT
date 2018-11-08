namespace FWT.Core.Entities
{
    public class TelegramSession 
    {
        public string HashId { get; set; }

        public byte[] Session { get; set; }
    }
}