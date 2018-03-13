using System;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.Controllers.Job.Index;
using Auth.FWT.API.Controllers.Statistics;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Job
{
    public class JobController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private IUserProvider _userProvider;

        public JobController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpPost]
        [Route("api/index")]
        public async Task Index(Guid jobId)
        {
            await _commandDispatcher.Dispatch(new IndexMessages.Command()
            {
                JobId = jobId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/ImportChatHistory")]
        public async Task ChatHistory(int chatId)
        {
            await _commandDispatcher.Dispatch(new StartImportingHistory.StartImportingChatHistory()
            {
                ChatId = chatId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/ImportUserChatHistory")]
        public async Task UserChatHistory(int userId)
        {
            await _commandDispatcher.Dispatch(new StartImportingHistory.StartImportingUserChatHistory()
            {
                UserId = userId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/ImportChannelHistory")]
        public async Task ChannelHistory(int channelId)
        {
            await _commandDispatcher.Dispatch(new StartImportingHistory.StartImportingChannalHistory()
            {
                ChannelId = channelId,
                CurrentUserId = _userProvider.CurrentUserId
            });
        }
    }
}