using System;
using TeleSharp.TL;

namespace Auth.FWT.API.Controllers.Jobs
{
    public class TelegramMessage
    {
        public TelegramMessage(TLMessage message)
        {
            Id = message.Id;
            CreatDateUTC = DateTimeOffset.FromUnixTimeSeconds(message.Date).UtcDateTime;
            EditDateUTC = message.EditDate.HasValue ? DateTimeOffset.FromUnixTimeSeconds(message.Date).UtcDateTime : (DateTime?)null;
            FromId = message.FromId;
            Message = message.Message;
        }

        public DateTime CreatDateUTC { get; set; }
        public DateTime? EditDateUTC { get; set; }
        public int? FromId { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
    }
}