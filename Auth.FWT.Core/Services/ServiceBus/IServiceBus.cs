using System.Threading.Tasks;

namespace Auth.FWT.Core.Services.ServiceBus
{
    public interface IServiceBus
    {
        Task SendToQueueAsync<TResult>(string name, TResult value);

        Task SendToTopicMessageAsync<TResult>(string name, TResult value);
    }
}