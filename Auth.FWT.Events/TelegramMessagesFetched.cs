using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.FWT.Events
{
    public class TelegramMessagesFetched
    {
        public int FetchedCount { get; set; }
        public Guid JobId { get; set; }
        public int Total { get; set; }
    }
}
