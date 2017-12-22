using System.Threading.Tasks;
using Auth.FWT.CQRS;
using Autofac;
using FluentValidation;

namespace Rws.Web.Core.CQRS
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IComponentContext _context;

        public QueryDispatcher(IComponentContext context)
        {
            _context = context;
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

            var handler = _context.Resolve<IQueryHandler<TQuery, TResult>>();
            return await handler.Handle(query);
        }
    }
}
