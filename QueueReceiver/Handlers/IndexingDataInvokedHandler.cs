using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.Events;
using Newtonsoft.Json;

namespace QueueReceiver.Handlers
{
    public class IndexingDataInvokedHandler : IEventHandler<IndexingDataInvoked>
    {
        private IUnitOfWork _unitOfWork;

        public IndexingDataInvokedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IndexingDataInvoked @event)
        {
            var result = await _unitOfWork.TelegramJobDataRepository.GetSingleAsync(@event.TelegramJobId);
            List<TelegramMessage> messages = JsonConvert.DeserializeObject<List<TelegramMessage>>(Encoding.UTF8.GetString(result.Data));
        }
    }
}