namespace Auth.FWT.Core.Entities
{
    public class TelegramJobData : BaseEntity<long>
    {
        public byte[] Data { get; set; }

        public virtual TelegramJob TelegramJob { get; set; }
    }
}