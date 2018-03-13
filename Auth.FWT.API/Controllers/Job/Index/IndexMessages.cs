using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Events;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Newtonsoft.Json;

namespace Auth.FWT.API.Controllers.Job.Index
{
    public class IndexMessages
    {
        public class Command : ICommand
        {
            public Guid JobId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task Execute(Command command)
            {
                var query = _unitOfWork.TelegramJobDataRepository.Query().Where(tjd => tjd.JobId == command.JobId);

                var dataRowsCount = await query.CountAsync();
                for (int i = 0; i < dataRowsCount; i++)
                {
                    var jobResult = await query.OrderBy(x => x.Id).Skip(i).FirstOrDefaultAsync();
                    List<TelegramMessage> messages = JsonConvert.DeserializeObject<List<TelegramMessage>>(Encoding.UTF8.GetString(jobResult.Data));
                }
            }
        }
    }
}