using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Events;
using Auth.FWT.Events;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;

namespace QueueReceiver
{
    public class Job
    {
        private IEventDispatcher _eventDispatcher;

        public Job(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async Task ProcessMessage([ServiceBusTrigger("processing")] BrokeredMessage message, TextWriter log)
        {
            string type = (string)message.Properties["type"];
            await ProcessMessage<TelegramJobCreated>(message, type);
            await ProcessMessage<TelegramMessagesFetched>(message, type);
            await ProcessMessage<IndexingDataInvoked>(message, type);
            await ProcessMessage<AllTelegramMessagesFetched>(message, type);
            await ProcessMessage<TelegramMessagesFetchingFailed>(message, type);
        }

        private async Task ProcessMessage<TEvent>(BrokeredMessage message, string type) where TEvent : IEvent
        {
            if (type == typeof(TEvent).FullName)
            {
                var model = message.GetBody<TEvent>(new DataContractJsonSerializer(typeof(TEvent)));
                await _eventDispatcher.Dispatch(model);
            }
        }
    }
}