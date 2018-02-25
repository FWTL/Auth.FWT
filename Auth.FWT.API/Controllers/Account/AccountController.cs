using Auth.FWT.API.Models;
using Auth.FWT.Core.Data;
using Auth.FWT.CQRS;
using Auth.FWT.Infrastructure.Telegram;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Auth.FWT.API.Controllers.Account
{
    public class AccountController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public AccountController(ICommandDispatcher commandDispatcher, IUnitOfWork uow, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        [Route("api/account/sendcode")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public async Task SendCode(string phoneNumber)
        {
            await _commandDispatcher.Dispatch(new SendCode.Command(phoneNumber));
        }

        [Route("api/account/reset")]
        [HttpGet]
        public void Reset()
        {
            AppUserSessionManager.Instance.UserSessionManager.Sessions.Clear();
        }
    }
}