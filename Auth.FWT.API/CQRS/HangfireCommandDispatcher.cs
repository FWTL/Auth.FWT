using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.CQRS;
using Hangfire;
using Hangfire.Server;

namespace Auth.FWT.API.CQRS
{
    public class HangfireCommandDispatcher
    {
        private ICommandDispatcher _commandDispatcher;

        public HangfireCommandDispatcher(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task Dispatch<TCommand>(TCommand command, PerformContext context) where TCommand : ICommand, IPerformContext
        {
            command.PerformContext = context;
            await _commandDispatcher.Dispatch(command);
        }
    }
}