using System.Net;
using System.Web.Http;
using Auth.FWT.API.Models;
using Auth.FWT.CQRS;
using Swashbuckle.Swagger.Annotations;

namespace Auth.FWT.API.Controllers.Login
{
    public class LoginController : ApiController
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public LoginController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public bool Login(string phoneNumber, string code)
        {
            return false;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ValidationResultModel))]
        public string TelegramCode(string phoneNumber)
        {
            return null;
        }
    }
}