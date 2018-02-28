using System.Collections.Generic;
using System.IO;
using Auth.FWT.API.Controllers.Events;
using Auth.FWT.Core.CQRS;
using Microsoft.Azure.WebJobs;
using static Auth.FWT.API.Controllers.Chat.GetUserChats;

namespace QueueReceiver
{
    public class Job
    {
        private IEventDispatcher _eventDispatcher;

        public Job(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public void ProcessQueueMessage([ServiceBusTrigger("redis")] UserChatsRefreshed value, TextWriter log)
        {
            _eventDispatcher.Dispatch(value);
        }
    }
}