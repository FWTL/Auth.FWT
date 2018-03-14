using System;

namespace Auth.FWT.Core.Entities
{
    public class TelegramJobData : BaseEntity<long>
    {
        public byte[] Data { get; set; }

        public Guid JobId { get; set; }
    }
}