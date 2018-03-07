using System.Threading.Tasks;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.CQRS;
using Autofac;
using FluentValidation;

namespace Rws.Web.Core.CQRS
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _context;
        private IServiceBus _serviceBus;

        public CommandDispatcher(IComponentContext context, IServiceBus serviceBus)
        {
            _context = context;
            _serviceBus = serviceBus;
        }

        public async Task<TResult> Dispatch<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            AbstractValidator<TCommand> validator;
            if (_context.TryResolve(out validator))
            {
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            var handler = _context.Resolve<ICommandHandler<TCommand, TResult>>();
            var result = await handler.Execute(command);

            foreach (var @event in handler.Events)
            {
                await @event.Send(_serviceBus);
            }

            return result;
        }

        public async Task Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            AbstractValidator<TCommand> validator;
            if (_context.TryResolve(out validator))
            {
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            var handler = _context.Resolve<ICommandHandler<TCommand>>();
            await handler.Execute(command);

            foreach (var @event in handler.Events)
            {
                await @event.Send(_serviceBus);
            }
        }
    }
}