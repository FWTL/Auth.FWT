using Auth.FWT.CQRS;
using GitGud.Web.Core.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

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
        public async Task<List<GetUserChats.Result>> Chats()
        {
            return await _queryDispatcher.Dispatch<GetUserChats.Query, List<GetUserChats.Result>>(new GetUserChats.Query(_userProvider.CurrentUserId));
        }
    }
}