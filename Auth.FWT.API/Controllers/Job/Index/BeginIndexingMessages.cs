using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Events;
using Auth.FWT.CQRS;
using Auth.FWT.Events;

namespace Auth.FWT.API.Controllers.Job.Index
{
    public class BeginIndexingMessages
    {
        public class Command : ICommand
        {
            public Guid JobId { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork, ICommandDispatcher commandDispatcher)
            {
                _unitOfWork = unitOfWork;
            }

            public List<IEvent> Events { get; set; } = new List<IEvent>();

            public async Task Execute(Command command)
            {
                var ids = await _unitOfWork.TelegramJobDataRepository.Query().Where(tjd => tjd.JobId == command.JobId).Select(tjd => tjd.Id).ToListAsync();
                foreach (var id in ids)
                {
                    Events.Add(new IndexingDataInvoked()
                    {
                        TelegramJobId = id
                    });
                }
            }
        }
    }
}