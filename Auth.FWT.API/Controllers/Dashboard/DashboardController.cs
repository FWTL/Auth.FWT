using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.SwaggerExtensions;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Dashboard
{
    public class DashboardController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUserProvider _userProvider;

        public DashboardController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpGet]
        [SwaggerDefaultValue("offset", 0)]
        [SwaggerDefaultValue("limit", 10)]
        public async Task<List<GetDashboard.Result>> Dashboard(int offset, int limit)
        {
            return await _queryDispatcher.Dispatch<GetDashboard.Query, List<GetDashboard.Result>>(new GetDashboard.Query()
            {
                UserId = _userProvider.CurrentUserId,
                Limit = limit,
                Offset = offset,
            });
        }
    }
}