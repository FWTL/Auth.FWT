using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.FWT.Core.Events;

namespace Auth.FWT.CQRS
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand
    {
        Task<TResult> Execute(TCommand command);

        List<IEvent> Events { get; }
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Execute(TCommand command);

        List<IEvent> Events { get; }
    }
}