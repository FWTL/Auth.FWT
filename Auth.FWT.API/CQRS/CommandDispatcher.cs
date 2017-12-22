using System.Threading.Tasks;
using Auth.FWT.CQRS;
using Autofac;
using FluentValidation;

namespace Rws.Web.Core.CQRS
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _context;

        public CommandDispatcher(IComponentContext context)
        {
            _context = context;
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
            return await handler.Execute(command);
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
        }
    }
}
