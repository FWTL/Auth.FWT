using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.Core.Providers;
using Auth.FWT.CQRS;

namespace Auth.FWT.API.Controllers.Chat
{
    public class ChatController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;

        private IQueryDispatcher _queryDispatcher;

        private IUserProvider _userProvider;

        public ChatController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IUserProvider userProvider)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _userProvider = userProvider;
        }

        [Authorize]
        [HttpGet]
        [Route("api/chats")]
        public async Task<List<GetUserChats.Result>> Chats(bool doRefresh)
        {
            return await _queryDispatcher.Dispatch<GetUserChats.Query, List<GetUserChats.Result>>(new GetUserChats.Query(_userProvider.CurrentUserId, doRefresh));
        }
    }
}
