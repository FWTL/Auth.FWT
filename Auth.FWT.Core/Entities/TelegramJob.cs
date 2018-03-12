using System;
using Auth.FWT.Core.Entities.Identity;
using static Auth.FWT.Core.Enums.Enum;

namespace Auth.FWT.Core.Entities
{
    public class TelegramJob : BaseEntity<long>
    {
        public DateTime CreatedDateUTC { get; set; }

        public DateTime LastStatusUpdateDateUTC { get; set; }

        public TelegramJobStatus Status { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        public virtual TelegramJobData TelegramJobData { get; set; }
    }
}