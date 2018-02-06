using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Auth.FWT.API.Models;
using Auth.FWT.CQRS;
using Swashbuckle.Swagger.Annotations;

namespace Auth.FWT.API.Controllers.Account
{
    public class AccountController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public AccountController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
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

        [HttpPost]
        [Route("api/account/login")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public async Task Login(string phoneNumber)
        {
            await _commandDispatcher.Dispatch(new SendCode.Command(phoneNumber));
        }
    }
}