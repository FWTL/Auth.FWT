using System.Threading.Tasks;
using Auth.FWT.Core.CQRS;
using Auth.FWT.Core.Services.ServiceBus;
using Auth.FWT.CQRS;
using Autofac;
using FluentValidation;

namespace Rws.Web.Core.CQRS
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IComponentContext _context;
        private IServiceBus _serviceBus;

        public QueryDispatcher(IComponentContext context, IServiceBus serviceBus)
        {
            _context = context;
            _serviceBus = serviceBus;
        }

        public async Task<TResult> Dispatch<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
        {
            AbstractValidator<TQuery> validator;
            if (_context.TryResolve(out validator))
            {
                var validationResult = validator.Validate(query);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }

            ICachableHandler<TQuery, TResult> cache;
            if (_context.TryResolve(out cache))
            {
                TResult result = await cache.Read(query);
                if (result != null)
                {
                    return result;
                }
            }

            var handler = _context.Resolve<IQueryHandler<TQuery, TResult>>();
            var queryResult = await handler.Handle(query);

            foreach (var @event in handler.Events)
            {
                await @event.Send(_serviceBus);
            }

            return queryResult;
        }
    }
}