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
        [Route("api/FetchChatHistory")]
        public async void FetchChatHistory(int chatId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingChatHistory()
            {
                ChatId = chatId,
                CurrentuserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/fetchUserChatHistory")]
        public async void FetchUserChatHistory(int userId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingUserChatHistory()
            {
                UserId = userId,
                CurrentuserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/fetchChannelHistory")]
        public async void FetchChannelHistory(int channelId)
        {
            await _commandDispatcher.Dispatch(new StartFeetchingHistory.StartFeetchingChannalHistory()
            {
                ChannelId = channelId,
                CurrentuserId = _userProvider.CurrentUserId
            });
        }
    }
}