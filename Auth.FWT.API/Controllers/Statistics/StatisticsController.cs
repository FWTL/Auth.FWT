using System.Threading.Tasks;
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
        [Route("api/fetchChatHistory")]
        public async Task FetchChatHistory(int chatId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingChatHistory()
            {
                ChatId = chatId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/fetchUserChatHistory")]
        public async Task FetchUserChatHistory(int userId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingUserChatHistory()
            {
                UserId = userId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/fetchChannelHistory")]
        public async Task FetchChannelHistory(int channelId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingChannalHistory()
            {
                ChannelId = channelId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }
    }
}