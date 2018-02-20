using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.Models;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Telegram;
using GitGud.API.Providers;
using GitGud.Web.Core.Providers;
using Swashbuckle.Swagger.Annotations;
using TLSharp.Core;
using TLSharp.Custom;

namespace Auth.FWT.API.Controllers.Account
{
    public class AccountController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;

        private IQueryDispatcher _queryDispatcher;
        private ISessionStore _ss;
        private ITelegramClient _tele;
        private IUserProvider _up;

        public AccountController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, ITelegramClient tele, IUserProvider up, TLSharp.Core.ISessionStore ss)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _tele = tele;
            _up = up;
            _ss = ss;
        }

        [HttpPost]
        [Route("api/account/sendcode")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public async Task SendCode(string phoneNumber)
        {
            await _commandDispatcher.Dispatch(new SendCode.Command(phoneNumber));
        }

        [Authorize]
        [Route("api/account/test")]
        [HttpGet]
        public async Task Test()
        {
            var id = _up.CurrentUserId;
            var session = AppUserSessionManager.Instance.UserSessionManager.Get(id.ToString(),_ss);
            var test = await _tele.GetUserDialogsAsync(session);
        }
    }
}