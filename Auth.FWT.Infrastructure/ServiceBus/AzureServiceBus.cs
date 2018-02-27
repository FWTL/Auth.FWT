using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Auth.FWT.Core;
using Auth.FWT.Core.Services.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Auth.FWT.Infrastructure.ServiceBus
{
    public class AzureServiceBus : IServiceBus, IDisposable
    {
        private Dictionary<string, QueueClient> _clients = new Dictionary<string, QueueClient>();
        private string _sessionId;

        public AzureServiceBus()
        {
            _sessionId = Guid.NewGuid().ToString("n");
        }

        public async Task SendToQueueAsync<TResult>(string name, TResult value)
        {
            var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value))))
            {
                ContentType = "application/json",
                SessionId = _sessionId
            };

            if (!_clients.ContainsKey(name))
            {
                _clients.Add(name, QueueClient.CreateFromConnectionString(ConfigKeys.AzureBusConnectionString, name));
            }

            await _clients[name].SendAsync(message);
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Value.Close();
            }
        }

        public Task SendToTopicMessageAsync<TResult>(string name, TResult value)
        {
            throw new NotImplementedException();
        }
    }
}