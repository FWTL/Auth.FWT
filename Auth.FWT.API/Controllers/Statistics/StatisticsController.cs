using System.Web.Http;
using Auth.FWT.API.Controllers.Statistics;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Reporting
{
    public class StatisticsController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUserProvider _userProvider;

        public StatisticsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpPost]
        [Route("api/fetchHistory")]
        public async void FetchHistory(int? chatId, int? channelId, int? userId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.Command()
            {
                ChatId = chatId,
                ChannelId = channelId,
                UserId = userId,
                CurrentuserId = _userProvider.CurrentUserId
            });
        }
    }
}