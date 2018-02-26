using System.Threading.Tasks;

namespace Auth.FWT.Core.Services.ServiceBus
{
    public interface IServiceBus
    {
        Task SendToQueueAsync<TResult>(string name, string label, TResult value);

        Task SendToTopicMessageAsync<TResult>(string name, string label, TResult value);
    }
}