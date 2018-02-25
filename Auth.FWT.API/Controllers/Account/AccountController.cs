using Auth.FWT.API.Models;
using Auth.FWT.Core.Data;
using Auth.FWT.Core.Entities.Identity;
using Auth.FWT.Core.Services.Telegram;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Telegram;
using GitGud.Web.Core.Providers;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace Auth.FWT.API.Controllers.Account
{
    public class AccountController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private ISessionStore _ss;
        private ITelegramClient _tele;
        private IUnitOfWork _uow;
        private IUserProvider _up;

        public AccountController(ICommandDispatcher commandDispatcher, IUnitOfWork uow, IQueryDispatcher queryDispatcher, ITelegramClient tele, IUserProvider up, TLSharp.Core.ISessionStore ss)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _tele = tele;
            _up = up;
            _ss = ss;
            _uow = uow;
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
        public async Task<TLAbsDialogs> Test()
        {
            var id = _up.CurrentUserId;
            var session = AppUserSessionManager.Instance.UserSessionManager.Get(id.ToString(), _ss);
            var test = await _tele.GetUserDialogsAsync(session);
            return test;
        }

        [Route("api/account/test2")]
        [HttpGet]
        public async Task<List<UserRole>> Test2()
        {
            return _uow.RoleRepository.GetAllIncluding().ToList();
        }

        [Route("api/account/reset")]
        [HttpGet]
        public void Reset()
        {
            AppUserSessionManager.Instance.UserSessionManager.Sessions.Clear();
        }
    }
}