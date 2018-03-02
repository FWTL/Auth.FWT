using System.IO;
using Auth.FWT.API.Controllers.Events;
using Auth.FWT.Core.CQRS;
using Microsoft.Azure.WebJobs;

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